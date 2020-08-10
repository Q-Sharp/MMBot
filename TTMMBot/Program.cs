using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using TTMMBot.Data;
using TTMMBot.Services;

namespace TTMMBot
{
    public class Program
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;

        public static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        private Program()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Debug,
                MessageCacheSize = 50,
            });

            _commands = new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Debug,
                CaseSensitiveCommands = false,
            });

            _client.Log += LogAsync;
            _commands.Log += LogAsync;
        }

        private static Task LogAsync(LogMessage message)
        {
            switch (message.Severity)
            {
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
            }

            Console.WriteLine($"{DateTime.Now, -19} [{message.Severity, 8}] {message.Source}: {message.Message} {message.Exception}");
            Console.ResetColor();

            return Task.CompletedTask;
        }

        private async Task MainAsync()
        {
            string token;
            var services = GetServices();
            var client = services.GetRequiredService<DiscordSocketClient>();
            services.GetRequiredService<CommandService>().Log += LogAsync;

            await services.GetRequiredService<IDatabaseService>().MigrateAsync();

            #if DEBUG
                token = "DiscordTokenDev";
            #else
                token = "DiscordToken";
            #endif

            await client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable(token));
            await client.StartAsync();

            // Here we initialize the logic required to register our commands.
            await services.GetRequiredService<CommandHandler>().InitializeAsync();

            await Task.Delay(Timeout.Infinite);
        }

        public IServiceProvider GetServices() => new ServiceCollection()
           .AddLogging()
           .AddDbContext<Context>()
           .AddSingleton<DiscordSocketClient>()
           .AddSingleton<CommandService>()
           .AddSingleton<CommandHandler>()
           .AddSingleton<IDatabaseService, DatabaseService>()
           .AddSingleton<NotionCSVImportService>()
           .BuildServiceProvider();
    }
}
