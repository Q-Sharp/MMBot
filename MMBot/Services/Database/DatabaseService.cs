using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MMBot.Data;
using MMBot.Data.Entities;
using MMBot.Helpers;
using MMBot.Services.Interfaces;

namespace MMBot.Services
{
    public class DatabaseService : IDatabaseService
    {
        private readonly Context _context;
        private ulong _guildId;

        public DatabaseService(Context context) => _context = context;
        public async Task MigrateAsync() => await _context?.MigrateAsync();
        public async Task SaveDataAsync() => await _context?.SaveChangesAsync(new CancellationToken());
        public async Task<GuildSettings> LoadGuildSettingsAsync() => await _context.GuildSettings.AsAsyncEnumerable().Where(x => x.GuildId == _guildId).FirstOrDefaultAsync();
        public void SetGuild(ulong id) => _guildId = id;

        
        public async Task<Clan> CreateClanAsync() => (await _context.AddAsync(new Clan{ GuildId = _guildId }, new CancellationToken())).Entity;
        public async Task<IList<Clan>> LoadClansAsync() => await _context.Clan.AsAsyncEnumerable().Where(x => x.GuildId == _guildId).ToListAsync();
        public void DeleteClan(Clan c) => _context.Remove(c);

        public async Task<Member> CreateMemberAsync() => (await _context.AddAsync(new Member { GuildId = _guildId }, new CancellationToken())).Entity;
        public async Task<IList<Member>> LoadMembersAsync() => await _context.Member.AsAsyncEnumerable().Where(x => x.GuildId == _guildId).ToListAsync();
        public void DeleteMember(Member m) => _context.Remove(m);

        public async Task<MMTimer> CreateTimerAsync() => (await _context.AddAsync(new MMTimer { GuildId = _guildId }, new CancellationToken())).Entity;
        public async Task<IList<MMTimer>> LoadTimerAsync() => await _context.Timer.AsAsyncEnumerable().Where(x => x.GuildId == _guildId).ToListAsync();
        public void DeleteTimer(MMTimer t) => _context.Remove(t);

        

        public async Task<Restart> AddRestart() => (await _context.AddAsync(new Restart(), new CancellationToken())).Entity;
        public async Task<Tuple<ulong, ulong>> ConsumeRestart()
        {
            try
            {
                var r = await _context.Restart.FirstOrDefaultAsync();
                var t = new Tuple<ulong, ulong>(r.Guild, r.Channel);
                _context.Restart.Remove(r);
                await _context.SaveChangesAsync(new CancellationToken());
                return t;
            }
            catch
            {
                return null;
            }
        }

        public async Task<Channel> CreateChannelAsync() => (await _context.AddAsync(new Channel(), new CancellationToken())).Entity;
        public async Task<IList<Channel>> LoadChannelsAsync() => await _context.Channel.ToListAsync();
        public void DeleteChannel(Channel c) => _context.Remove(c);

        public async Task CleanDB()
        {
            var c = await LoadClansAsync();
            var m = await LoadMembersAsync();

            if(c == null || c.Count() == 0 || m == null || m.Count() == 0)
                return;

            m.Where(x => x.ClanId == 0 || x.Name == string.Empty).ForEach(x =>
            {
                var id = x.ClanId;
                _context.Remove(x);

                if(id.HasValue)
                    _context.Remove(c.FirstOrDefault(y => y.Id == id));
                _context.SaveChanges();
            });
        }
    }
}
