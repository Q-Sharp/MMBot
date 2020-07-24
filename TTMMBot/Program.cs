using TTMMBot.Data;
using TTMMBot.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TTMMBot
{
    public class Program
    {
        private const string dbname = "TTMMBot.db"; 
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        //private readonly IServiceProvider _services;

        public static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        private Program()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
                MessageCacheSize = 50,
            });

            _commands = new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Info,
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
            var services = GetServices();
            var client = services.GetRequiredService<DiscordSocketClient>();
            services.GetRequiredService<CommandService>().Log += LogAsync;

            await client.LoginAsync(TokenType.Bot, /*Environment.GetEnvironmentVariable("DiscordToken")*/"NjY3OTkwMzcwMDA0NzYyNjM0.XiKwgg.DaT6K25l0jzwDwxV8Faf9SrP_J4");
            await client.StartAsync();

            // Here we initialize the logic required to register our commands.
            await services.GetRequiredService<CommandHandlingService>().InitializeAsync();

            await Task.Delay(Timeout.Infinite);
        }

         public IServiceProvider GetServices() => new ServiceCollection()
            .AddLogging()
            .AddDbContext<Context>(options => options.UseSqlite($"Data Source={dbname}"))
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton<CommandService>()
            .AddSingleton<CommandHandlingService>()
            .AddSingleton<IDatabaseService, DatabaseService>()
            .BuildServiceProvider();
    }
}
