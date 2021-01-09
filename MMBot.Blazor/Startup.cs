using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord.OAuth2;
using Discord.WebSocket;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MMBot.Blazor.Services;
using MMBot.Data;
using MMBot.Data.Services;
using MMBot.Data.Services.Interfaces;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace MMBot.Blazor
{
    public class Startup
    {
        private const string _logTemplate = "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            //Log.Logger = new LoggerConfiguration()
            //     .Enrich.FromLogContext()
            //     .WriteTo.Console(theme: AnsiConsoleTheme.Literate, outputTemplate: _logTemplate, restrictedToMinimumLevel: LogEventLevel.Verbose)
            //     .WriteTo.File(path: Configuration.GetValue<string>("logpath"), outputTemplate: _logTemplate, rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: LogEventLevel.Warning)
            //     .CreateLogger();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(c => c.ClearProviders().AddSerilog(Log.Logger));

            services.AddRazorPages();
            services.AddServerSideBlazor().AddHubOptions(c => c.EnableDetailedErrors = true);

            //services.AddDbContext<Context>(o => o.UseSqlite($"Data Source={_fullDbPath}"))
            //        .AddScoped<IDatabaseService, DatabaseService>();

            var c = services.Count;

            services.AddAuthentication(opt =>
            {
                opt.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                opt.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = DiscordDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddDiscord(x =>
            {
                x.AppId = Configuration["Discord:AppId"];
                x.AppSecret = Configuration["Discord:AppSecret"];

                x.SaveTokens = true;
                x.Scope.Add("guilds");
                x.Validate();
            });

            services.AddTransient<AccountService>();

            services.AddHttpClient();
            services.AddTransient<DiscordSocketClient>();
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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            
            app.UseRouting()
               .UseAuthentication()
               .UseEndpoints(endpoints =>
                {
                    endpoints.MapBlazorHub();
                    endpoints.MapFallbackToPage("/_Host");
                    endpoints.MapDefaultControllerRoute();
                });
        }
    }
}
