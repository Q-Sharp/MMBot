namespace MMBot.DSharp.Modules;

public abstract class MMBotModule : ApplicationCommandModule
{
    protected IDatabaseService _databaseService;
    protected IGuildSettingsService _guildSettingsService;
    protected readonly InteractionContext _ctx;
    private readonly Task<GuildSettings> _guildSettings;

    public MMBotModule(IDatabaseService databaseService/*, IGuildSettingsService guildSettingsService, InteractionContext ctx*/)
    {
        _databaseService = databaseService;
        //_guildSettingsService = guildSettingsService;
        //_ctx = ctx;

        //if (ctx?.Guild?.Id is not null)
        //    _guildSettings = guildSettingsService?.GetGuildSettingsAsync(ctx.Guild.Id);
    }

    public Task<GuildSettings> GetGuildSettings() => _guildSettings;
}
