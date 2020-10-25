using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using TTMMBot.Data;
using TTMMBot.Services;
using TTMMBot.Services.CommandHandler;
using TTMMBot.Services.GoogleForms;
using TTMMBot.Services.IE;
using TTMMBot.Services.Interfaces;
using TTMMBot.Services.MemberSort;

namespace TTMMBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                 .Enrich.FromLogContext()
                 .WriteTo.Console(theme: AnsiConsoleTheme.Literate, outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u4}] {Message:lj}{NewLine}{Exception}")
                 .CreateLogger();

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;

            using var h = CreateHostBuilder(args)?.Build();
            h?.Run();
            Log.CloseAndFlush();
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e) => Log.Logger.Information("Exiting!");

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
                Log.Logger.Error($"Unhandled exception: {ex.Message}");

            if (e.IsTerminating)
                Log.Logger.Error("Terminating!");
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
           Host.CreateDefaultBuilder(args)
                .ConfigureLogging(loggingBuilder => loggingBuilder.ClearProviders().AddSerilog(dispose: true))
                .UseSystemd()
                .ConfigureAppConfiguration((hostContext, configBuilder) =>
                {
                    configBuilder.AddEnvironmentVariables("MMBot_");
                })
                .ConfigureServices((hostContext, services) =>
                {
                    var dsc = new DiscordSocketClient(new DiscordSocketConfig
                    {
                        LogLevel = LogSeverity.Warning,
                        MessageCacheSize = 1000,
                        DefaultRetryMode = RetryMode.AlwaysRetry
                    });

                    var cs = new CommandService(new CommandServiceConfig
                    {
                        LogLevel = LogSeverity.Warning,
                        CaseSensitiveCommands = false,
                        DefaultRunMode = RunMode.Async,
                        SeparatorChar = ' '
                    });


                    var hc = services.AddHttpClient("forms", c =>
                    {
                        c.BaseAddress = new Uri("https://docs.google.com");
                        c.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
                        c.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.75 Safari/537.36");
                    });

                     services.AddHostedService<DiscordWorker>()
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
                        .BuildServiceProvider();
                });
    }
}
