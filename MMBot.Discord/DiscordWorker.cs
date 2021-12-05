using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MMBot.Data.Services.Interfaces;
using MMBot.Discord.Services.Interfaces;

namespace MMBot.Discord
{
    public class DiscordWorker : BackgroundService
    {
        private readonly IServiceProvider _sp;
        private readonly ILogger<DiscordWorker> _logger;
        private readonly IConfiguration _config;
        private readonly IHostEnvironment _env;

        public DiscordWorker(IServiceProvider sp, ILogger<DiscordWorker> logger, IConfiguration config, IHostEnvironment env)
        {
            _sp = sp;
            _logger = logger;
            _config = config;
            _env = env;
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
                _logger.LogError(e.Message);
                ads.Truncate();
                await ads.Restart();
                return;
            }

            var client = _sp.GetRequiredService<DiscordSocketClient>();
            _sp.GetRequiredService<CommandService>().Log += LogAsync;
            client.Log += LogAsync;

            var token = _env.IsDevelopment() ? _config.GetSection("Discord").GetValue<string>("DevToken") 
                : _config.GetSection("Discord").GetValue<string>("Token");

#if DEBUG
            token = _config.GetSection("Discord").GetValue<string>("DevToken");
#endif

            await client.LoginAsync(TokenType.Bot, token);
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
                    _logger.LogError(message.Message);
                    break;

                case LogSeverity.Warning:
                    _logger.LogWarning(message.Message);
                    break;

                case LogSeverity.Info:
                    _logger.LogInformation(message.Message);
                    break;

                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    _logger.LogDebug(message.Message);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
