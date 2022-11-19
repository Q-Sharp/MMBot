using MMBot.Data;
using MMBot.Discord.Services.Interfaces;

namespace MMBot.Discord.Services.GuildSettings;

public class GuildSettingsService : IGuildSettingsService
{
    private readonly Context _dbcontext;

    public GuildSettingsService(Context dbcontext) => _dbcontext = dbcontext;

    public async Task<Data.Contracts.Entities.GuildSettings> GetGuildSettingsAsync(ulong guildId)
    {
        var settings = _dbcontext.GuildSettings.FirstOrDefault(x => x.GuildId == guildId) ?? await CreateNewSettingsAsync(guildId);

        return settings;
    }

    private async Task<Data.Contracts.Entities.GuildSettings> CreateNewSettingsAsync(ulong id)
    {
        var gs = (await _dbcontext.AddAsync(new Data.Contracts.Entities.GuildSettings()
        {
            GuildId = id,
            Prefix = "m.",
            FileName = "export.csv",
            ClanSize = 20,
            MemberMovementQty = 3,
        }))
        .Entity;

        _ = (_dbcontext?.SaveChanges());
        return gs;
    }
}
