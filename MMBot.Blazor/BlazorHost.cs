using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MMBot.Blazor.Data;
using MMBot.Blazor.Helpers;
using MMBot.Blazor.Services;
using MMBot.Data;
using MMBot.Data.Services;
using MMBot.Data.Services.Interfaces;
using MMBot.Services.Database;
using MudBlazor.Services;
using Serilog;

namespace MMBot.Blazor
{
    public static class BlazorHost
    {
        public static IHostBuilder CreateHostBuilder(string[] args)
            => Host.CreateDefaultBuilder(args)
                   .UseSystemd()
                   .ConfigureAppConfiguration((hostContext, configBuilder) =>
                   {
                       configBuilder.AddEnvironmentVariables("MMBot_")
                                    .AddCommandLine(args);
                   })
                   .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
                   .UseSerilog((h, l) => l.ReadFrom.Configuration(h.Configuration));
    }

    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(c => c.ClearProviders().AddSerilog(Log.Logger));

            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddRouting();

            var connectionString = Configuration.GetConnectionString("Context");
            
            services.AddDbContext<Context>(o => o.UseNpgsql(connectionString))
                    .AddScoped<IDatabaseService, DatabaseService>();

            services.AddDataProtection()
                    .UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration()
                    {
                        EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
                        ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
                    })
                    .SetApplicationName("MMBot");

            services.AddAuthentication(opt =>
            {
                opt.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                opt.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = "Discord";
            })
            .AddCookie(c =>
            {
                c.Cookie.SameSite = SameSiteMode.Strict;
            })
            .AddDiscord(x =>
            {
                x.ClientId = Configuration["Discord:AppId"];
                x.ClientSecret = Configuration["Discord:AppSecret"];

                x.Events = new OAuthEvents
                {
                    OnCreatingTicket = async context =>
                    {
                        var guildClaim = await DiscordHelpers.GetGuildClaims(context);
                        context.Identity.AddClaim(guildClaim);
                    },
                    OnAccessDenied = context =>
                    {
                        context.AccessDeniedPath = PathString.FromUriComponent("/");
                        context.ReturnUrlParameter = string.Empty;
                        return Task.CompletedTask;
                    }
                };

                x.SaveTokens = true;
                x.Validate();
            });

            services.AddAuthorization(c =>
            {
                c.InvokeHandlersAfterFailure = false;
            });

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.KnownProxies.Add(IPAddress.Any);
                options.ForwardedHeaders = ForwardedHeaders.All;
            });

            services.AddSession();
            services.AddHttpContextAccessor();
            services.AddScoped<IAccountService, AccountService>()
                    .AddScoped<IDCUser, DCUser>();

            services.AddSingleton<StateContainer>();
            services.AddScoped<BlazorDatabaseService>();

            services.AddCRUDViewModels();
            services.AddMudServices();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSerilogRequestLogging();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseForwardedHeaders()
                   .UseCertificateForwarding()
                   .UseExceptionHandler("/Error")
                   .UseHsts()
                   .UseCookiePolicy();
            }

            app.UseHttpsRedirection()
               .UseStaticFiles()
               .UseRouting()
               .UseAuthentication()
               .UseAuthorization()
               .UseSession()
               .UseEndpoints(endpoints =>
               {
                    endpoints.MapBlazorHub();
                    endpoints.MapFallbackToPage("/_Host");
                    endpoints.MapDefaultControllerRoute();
               });
        }
    }
}
