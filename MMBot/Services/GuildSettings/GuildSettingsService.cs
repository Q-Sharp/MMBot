using System;
using System.Linq;
using System.Threading.Tasks;
using MMBot.Data;
using MMBot.Data.Entities;
using MMBot.Services.Interfaces;

namespace MMBot.Services
{
    public class GuildSettingsService : IGuildSettingsService
    {
        private readonly Context _dbcontext;

        public GuildSettingsService(Context dbcontext) 
        {
            _dbcontext = dbcontext;
        }

        public async Task<GuildSettings> GetGuildSettingsAsync(ulong guildId)
            => await _dbcontext.GuildSettings.FirstOrDefaultAsync(x => x.GuildId == guildId) ?? (await CreateNewSettingsAsync(guildId));

        private async Task<GuildSettings> CreateNewSettingsAsync(ulong id)
        {
            var gs = (await _dbcontext.AddAsync(new GuildSettings()
            {
                GuildId = id,
                Prefix = "m.",
                WaitForReaction = TimeSpan.FromMinutes(5),
                FileName = "export.csv",
                ClanSize = 20,
                MemberMovementQty = 3
            }))
            .Entity;

            _dbcontext?.SaveChanges();
            return gs;
        }
    }
}
