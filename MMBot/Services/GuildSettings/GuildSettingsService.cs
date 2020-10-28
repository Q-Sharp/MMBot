using System;
using System.Linq;
using MMBot.Data;
using MMBot.Data.Entities;
using MMBot.Services.Interfaces;

namespace MMBot.Services
{
    public class GuildSettingsService : IGuildSettingsService
    {
        private readonly Context _dbcontext;
        private GuildSettings _gs;

        public GuildSettingsService(Context dbcontext) 
            => _dbcontext = dbcontext;

        // Discord
        public string Prefix => _gs?.Prefix ?? ".m";
        public TimeSpan WaitForReaction => _gs?.WaitForReaction ?? TimeSpan.FromMinutes(5);

        // Filesystem
        public string FileName => _gs?.FileName ?? "export.csv";

        public ulong GuildId => _gs?.GuildId ?? 0;

        // InGame
        public int ClanSize => _gs?.ClanSize ?? 20;
        public int MemberMovementQty => _gs?.MemberMovementQty ?? 3;

        private void CreateNewSettings(ulong id)
        {
            _gs = _dbcontext.Add(new GuildSettings()
            {
                GuildId = id,
                Prefix = "m.",
                WaitForReaction = TimeSpan.FromMinutes(5),
                FileName = "export.csv",
                ClanSize = 20,
                MemberMovementQty = 3
            })
            .Entity;

            _dbcontext?.SaveChanges();
        }

        public void SetGuild(ulong id)
        {
             _gs = _dbcontext.GuildSettings.FirstOrDefault(x => x.GuildId == id);

            if(_gs == null)
                CreateNewSettings(id);
        }
    }
}
