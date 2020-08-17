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

            using var h = CreateHostBuilder(args)?.Build();
            h?.Run();
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
                        LogLevel = LogSeverity.Debug,
                        MessageCacheSize = 100,                       
                    });

                    var cs = new CommandService(new CommandServiceConfig
                    {
                        LogLevel = LogSeverity.Debug,
                        CaseSensitiveCommands = false,
                    });

                    services
                         .AddHostedService<DiscordWorker>()    
                         .AddSingleton<GlobalSettings>()
                         .AddDbContext<Context>()
                         .AddTriggers()
                         .AddSingleton(dsc)
                         .AddSingleton(cs)
                         .AddSingleton<CommandHandler>()
                         .AddSingleton<IDatabaseService, DatabaseService>()
                         .AddSingleton<NotionCSVImportService>()
                         .AddSingleton<AdminService>()
                         .BuildServiceProvider();
                });
    }
}
