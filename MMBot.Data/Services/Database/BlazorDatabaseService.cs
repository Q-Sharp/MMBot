namespace MMBot.Data.Services.Database;

public class BlazorDatabaseService : IBlazorDatabaseService
{
    private readonly Context _ctx;

    public BlazorDatabaseService(Context dataContext) => _ctx = dataContext;

    public IEnumerable<Guild> GetAllGuilds()
        => _ctx.GuildSettings
            .Select(x => new Guild(x.GuildId, x.GuildName))
            .Distinct()
            .ToList();
}
