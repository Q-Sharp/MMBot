using System;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using EntityFrameworkCore.Triggers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using TTMMBot.Data;
using TTMMBot.Services;
using TTMMBot.Services.GoogleForms;
using TTMMBot.Services.Interfaces;

namespace TTMMBot
{
    public class Program
    {
        private static IServiceProvider _services;

        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                 .Enrich.FromLogContext()
                 .WriteTo.Console(theme: AnsiConsoleTheme.Literate, outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u4}] {Message:lj}{NewLine}{Exception}")
                 .CreateLogger();

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;

            using var h = CreateHostBuilder(args)?.Build();
            _services = h.Services;
            h?.Run();
            Log.CloseAndFlush();
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            if (_services != null)
            {
                var discordClient = _services.GetService<DiscordSocketClient>();
                if (discordClient != null)
                {
                    discordClient.LogoutAsync().Wait();
                    discordClient.StopAsync().Wait();
                }
            }

            Log.Logger.Information("Exiting!");
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
                Log.Logger.Error($"Unhandled exception: {ex.Message}");

            if (e.IsTerminating)
                Log.Logger.Error("Terminating!");
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
           //Host.CreateDefaultBuilder(args)
           new HostBuilder()
                .ConfigureLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true))
                .UseSystemd()
                .ConfigureServices((hostContext, services) =>
                {
                    var dsc = new DiscordSocketClient(new DiscordSocketConfig
                    {
                        LogLevel = LogSeverity.Warning,
                        MessageCacheSize = 300,                       
                    });

                    var cs = new CommandService(new CommandServiceConfig
                    {
                        LogLevel = LogSeverity.Warning,
                        CaseSensitiveCommands = false,
                        SeparatorChar = ' '
                    });


                    var hc = services.AddHttpClient("forms", c =>
                    {
                        c.BaseAddress = new Uri("https://docs.google.com");
                        c.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
                        c.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.75 Safari/537.36");
                    });

                    services.AddHostedService<DiscordWorker>()
                        .AddSingleton<GlobalSettingsService>()
                        .AddDbContext<Context>()
                        .AddTriggers()
                        .AddSingleton<IGlobalSettingsService, GlobalSettingsService>()
                        .AddSingleton(dsc)
                        .AddSingleton(cs)
                        .AddTransient<IGoogleFormsService, GoogleFormsService>()
                        .AddSingleton<ICommandHandler, CommandHandler>()
                        .AddSingleton<IGlobalSettingsService, GlobalSettingsService>()
                        .AddTransient<IDatabaseService, DatabaseService>()
                        .AddTransient<INotionCsvService, NotionCsvService>()
                        .AddTransient<IAdminService, AdminService>()
                        .AddTransient<IMemberSortService, MemberSortService>()
                        .BuildServiceProvider();
                });
    }
}
