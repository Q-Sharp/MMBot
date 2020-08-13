using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Net.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TTMMBot.Data;
using TTMMBot.Services;

namespace TTMMBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using var h = CreateHostBuilder(args)?.Build();
            h?.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
           Host.CreateDefaultBuilder(args)
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

                   services.AddHostedService<DiscordWorker>()
                       .AddLogging()
                       .AddDbContext<Context>()
                       .AddSingleton(dsc)
                       .AddSingleton(cs)
                       .AddSingleton<CommandHandler>()
                       .AddSingleton<IDatabaseService, DatabaseService>()
                       .AddSingleton<NotionCSVImportService>()
                       .BuildServiceProvider();
               });
    }
}
