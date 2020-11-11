using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using MMBot.Data;
using MMBot.Services;
using MMBot.Services.CommandHandler;
using MMBot.Services.GoogleForms;
using MMBot.Services.IE;
using MMBot.Services.Interfaces;
using MMBot.Services.MemberSort;
using MMBot.Services.Timer;
using System.IO;
using Serilog.Events;

namespace MMBot
{
    public class Program
    {
        private const string logTemplate = "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u4}] {Message:lj}{NewLine}{Exception}";

        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                 .Enrich.FromLogContext()
                 .WriteTo.Console(theme: AnsiConsoleTheme.Literate, outputTemplate: logTemplate, restrictedToMinimumLevel: LogEventLevel.Verbose)
                 .WriteTo.File(path: Path.Combine(Environment.CurrentDirectory, "mmbot.log"), outputTemplate: logTemplate, rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: LogEventLevel.Warning)
                 .CreateLogger();

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;

            try
            {
                using var h = CreateHostBuilder(args)?.Build();
                h?.Run();
            }
            catch(Exception e)
            {
                Log.Fatal(e, logTemplate);
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e) 
            => Log.Logger.Information("Exiting!");

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
                Log.Logger.Error($"Unhandled exception: {ex.Message}");

            if (e.IsTerminating)
                Log.Logger.Error("Terminating!");
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
           Host.CreateDefaultBuilder(args)
                .ConfigureLogging(x => x.ClearProviders().AddSerilog(Log.Logger))
                .UseSystemd()
                .ConfigureAppConfiguration((hostContext, configBuilder) =>
                {
                    configBuilder.AddEnvironmentVariables("MMBot_");
                })
                .ConfigureServices((hostContext, services) =>
                {
                    var dsc = new DiscordSocketClient(new DiscordSocketConfig
                    {
                        LogLevel = LogSeverity.Debug,
                        MessageCacheSize = 1000,
                        DefaultRetryMode = RetryMode.AlwaysRetry,
                        HandlerTimeout = null,
                        LargeThreshold = 250
                    });

                    var cs = new CommandService(new CommandServiceConfig
                    {
                        LogLevel = LogSeverity.Debug,
                        CaseSensitiveCommands = false,
                        DefaultRunMode = RunMode.Async,
                        SeparatorChar = ' ',
                        IgnoreExtraArgs = true
                    });

                    var hc = services.AddHttpClient("forms", c =>
                    {
                        c.BaseAddress = new Uri("https://docs.google.com");
                        c.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
                        c.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.75 Safari/537.36");
                    });

                    services
                        .AddHostedService<DiscordWorker>()
                        .AddSingleton<GuildSettingsService>()
                        .AddDbContext<Context>()
                        .AddSingleton<IGuildSettingsService, GuildSettingsService>()
                        .AddSingleton(dsc)
                        .AddSingleton(cs)
                        .AddScoped<IGoogleFormsService, GoogleFormsService>()
                        .AddSingleton<ICommandHandler, CommandHandler>()
                        .AddScoped<IGuildSettingsService, GuildSettingsService>()
                        .AddScoped<IDatabaseService, DatabaseService>()
                        .AddScoped<ICsvService, CsvService>()
                        .AddScoped<IJsonService, JsonService>()
                        .AddScoped<IAdminService, AdminService>()
                        .AddScoped<IMemberSortService, MemberSortService>()
                        .AddSingleton<InteractiveService, InteractiveService>()
                        .AddSingleton<ITimerService, TimerService>()
                        .BuildServiceProvider();
                });
    }
}
