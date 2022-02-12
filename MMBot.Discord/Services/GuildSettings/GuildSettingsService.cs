using Microsoft.EntityFrameworkCore;
using MMBot.Data;
using MMBot.Discord.Services.Interfaces;

namespace MMBot.Discord.Services.GuildSettings;

public class GuildSettingsService : IGuildSettingsService
{
    private readonly Context _dbcontext;

    public GuildSettingsService(Context dbcontext)
    {
        _dbcontext = dbcontext;
    }

    public async Task<Data.Entities.GuildSettings> GetGuildSettingsAsync(ulong guildId)
        => await _dbcontext.GuildSettings.FirstOrDefaultAsync(x => x.GuildId == guildId) ?? await CreateNewSettingsAsync(guildId);

    private async Task<Data.Entities.GuildSettings> CreateNewSettingsAsync(ulong id)
    {
        var gs = (await _dbcontext.AddAsync(new Data.Entities.GuildSettings()
        {
            GuildId = id,
            Prefix = "m.",
            FileName = "export.csv",
            ClanSize = 20,
            MemberMovementQty = 3
        }))
        .Entity;

        _dbcontext?.SaveChanges();
        return gs;
    }
}
