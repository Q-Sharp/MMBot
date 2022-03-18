using MMBot.Data.Contracts;

namespace MMBot.Data.Services.Database;

public class BlazorDatabaseService : IBlazorDatabaseService
{
    private readonly Context _ctx;

    public BlazorDatabaseService(Context dataContext) => _ctx = dataContext;

    public IList<Tuple<ulong, string>> GetAllGuilds()
        => _ctx.GuildSettings.AsQueryable()
            .Select(x => Tuple.Create(x.GuildId, x.GuildName))
            .Distinct()
            .ToList();
}
