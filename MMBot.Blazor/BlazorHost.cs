using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MMBot.Blazor.Helpers;
using MMBot.Blazor.Services;
using MMBot.Data;
using MMBot.Data.Services;
using MMBot.Data.Services.Interfaces;
using MudBlazor.Services;
using MMBot.Blazor.Data;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;

namespace MMBot.Blazor
{
    public static class BlazorHost
    {
        public static IHostBuilder CreateHostBuilder(string[] args) 
            => Host.CreateDefaultBuilder(args)
                   .UseSystemd()
                   .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
                   .UseSerilog();
    }

    public class Startup
    {
        private const string _logTemplate = "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}";

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
            .AddCookie()
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
                    }
                };

                x.SaveTokens = true;
                x.Validate();
            });

            services.AddSession();
            services.AddHttpContextAccessor();
            services.AddScoped<IAccountService, AccountService>()
                    .AddScoped<IDCUser, DCUser>();

            services.AddSingleton<StateContainer>();

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
                app.UseForwardedHeaders();

                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
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
