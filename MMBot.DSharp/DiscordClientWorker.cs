using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace MMBot.DSharp;

public class DiscordClientWorker : IHostedService
{
    private readonly DiscordClient _discordClient;
    private readonly IServiceProvider _sp;

    public DiscordClientWorker(DiscordClient discordClient, IServiceProvider sp)
    {
        _discordClient = discordClient;
        _sp = sp;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var intr = _discordClient.UseInteractivity(new InteractivityConfiguration
        {
            PollBehaviour = PollBehaviour.KeepEmojis,
            ResponseBehavior = InteractionResponseBehavior.Ack,
            Timeout = TimeSpan.FromSeconds(30)
        });

        var slash = _discordClient.UseSlashCommands(new SlashCommandsConfiguration
        {
            Services = _sp
        });
        slash.RegisterCommands<SlashCommandHandler>();

        var da = new DiscordActivity("TEST", ActivityType.Watching);
        var us = UserStatus.DoNotDisturb;

        await _discordClient.ConnectAsync(da, us);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _discordClient.DisconnectAsync();
    }
}