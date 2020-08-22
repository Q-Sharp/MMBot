using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using TTMMBot.Services;

namespace TTMMBot
{
    public class DiscordWorker : BackgroundService
    {
        private readonly IServiceProvider _sp;
        private readonly Microsoft.Extensions.Logging.ILogger _logger;

        public DiscordWorker(IServiceProvider sp, ILogger<DiscordWorker> logger)
        {
            _sp = sp;
            _logger = logger;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTime.Now);
                await InitAsync();
                await Task.Delay(Timeout.Infinite);
            }
        }

        public async override Task StartAsync(CancellationToken cancellationToken) => await base.StartAsync(cancellationToken);

        public async override Task StopAsync(CancellationToken cancellationToken) => await Task.Run(() => Dispose());

        public async Task InitAsync()
        {
            var dbs = _sp.GetRequiredService<IDatabaseService>();
            try
            {
                await dbs.MigrateAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Migration failed");
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
            client.Log += LogAsync;

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
                    Log.Error(message.Message);
                    break;
                case LogSeverity.Warning:
                    Log.Warning(message.Message);
                    break;
                case LogSeverity.Info:
                    Log.Information(message.Message);
                    break;
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    Log.Verbose(message.Message);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
