using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TTMMBot.Services;

namespace TTMMBot
{
    public class DiscordWorker : BackgroundService
    {
        private static ILogger<DiscordWorker> _logger;
        private readonly IServiceProvider _sp;

        public DiscordWorker(ILogger<DiscordWorker> logger, IServiceProvider sp)
        {
            _logger = logger;
            _sp = sp;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await InitilizeAsync();
                await Task.Delay(Timeout.Infinite);
            }
        }

        public override async Task StartAsync(CancellationToken cancellationToken) => await base.StartAsync(cancellationToken);

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.Run(() => Dispose());
        }

        public async Task InitilizeAsync()
        {
            var dbs = _sp.GetRequiredService<IDatabaseService>();
            try
            {
                await dbs.MigrateAsync();
            }
            catch (Exception e)
            {
                var lm = new LogMessage(LogSeverity.Error, "DatabaseService", "Migration failed", e);
                await LogAsync(lm);
                return;
            }

            string token;

#if DEBUG
            token = "DiscordTokenDev";
#else
            token = "DiscordToken";
#endif
            var client = _sp.GetRequiredService<DiscordSocketClient>();
            _sp.GetRequiredService<CommandService>().Log += LogAsync;
            await client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable(token));
            await client.StartAsync();

            // Here we initialize the logic required to register our commands.
            await _sp.GetRequiredService<CommandHandler>().InitializeAsync();
        }

        public static Task LogAsync(LogMessage message)
        {
            switch (message.Severity)
            {
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    _logger.LogError(message.Message);
                    break;
                case LogSeverity.Warning:
                    _logger.LogWarning(message.Message);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Info:
                    _logger.LogInformation(message.Message);
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    _logger.LogDebug(message.Message);
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
            }

            Console.WriteLine($"{DateTime.Now,-19} [{message.Severity,8}] {message.Source}: {message.Message} {message.Exception}");
            Console.ResetColor();

            return Task.CompletedTask;
        }
    }
}
