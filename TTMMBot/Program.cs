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

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledExceptionAsync;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExitAsync;

            using var h = CreateHostBuilder(args)?.Build();
            _services = h.Services;
            h?.Run();
            Log.CloseAndFlush();
        }

        private static async void CurrentDomain_ProcessExitAsync(object sender, EventArgs e)
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
            await Console.Out.WriteLineAsync("Exiting!").ConfigureAwait(false);
        }

        private static async void CurrentDomain_UnhandledExceptionAsync(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
                await Console.Out.WriteLineAsync($"Unhandled exception: {ex.Message}").ConfigureAwait(false);

            if (e.IsTerminating)
                await Console.Out.WriteLineAsync("Terminating!").ConfigureAwait(false);
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
