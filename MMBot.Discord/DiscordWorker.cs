using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using MMBot.Discord.Services.Interfaces;
using MMBot.Data.Services.Interfaces;

namespace MMBot.Discord
{
    public class DiscordWorker : BackgroundService
    {
        private readonly IServiceProvider _sp;
        private readonly ILogger<DiscordWorker> _logger;
        private readonly IConfiguration _config;

        public DiscordWorker(IServiceProvider sp, ILogger<DiscordWorker> logger, IConfiguration config)
        {
            _sp = sp;
            _logger = logger;
            _config = config;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTime.UtcNow);
                await InitAsync();
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
        }

        public async override Task StartAsync(CancellationToken cancellationToken) => await base.StartAsync(cancellationToken);

        public async override Task StopAsync(CancellationToken cancellationToken) => await Task.Run(() => Dispose(), cancellationToken);

        public async Task InitAsync()
        {
            var dbs = _sp.GetRequiredService<IDatabaseService>();
            var ads = _sp.GetRequiredService<IAdminService>();

            try
            {
                await dbs.MigrateAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e, "Migration failed");
                ads.DeleteDb();
                await ads.Restart();
                return;
            }

            var client = _sp.GetRequiredService<DiscordSocketClient>();
            _sp.GetRequiredService<CommandService>().Log += LogAsync;
            client.Log += LogAsync;

            var token =
#if DEBUG
            //_config.GetProperty("DiscordTokenDev");
            "DiscordTokenDev";
#else
            //_config.GetProperty("DiscordToken");
            "DiscordToken";
#endif

            await client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable(token));
            await client.StartAsync();

            var ch = _sp.GetRequiredService<ICommandHandler>();
            await ch?.InitializeAsync();
        }

        public Task LogAsync(LogMessage message)
        {
            switch (message.Severity)
            {
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    _logger.LogError(message.Message, message.Exception);
                    break;

                case LogSeverity.Warning:
                    _logger.LogWarning(message.Message, message.Exception);
                    break;

                case LogSeverity.Info:
                    _logger.LogInformation(message.Message, message.Exception);
                    break;

                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    _logger.LogDebug(message.Message, message.Exception);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
