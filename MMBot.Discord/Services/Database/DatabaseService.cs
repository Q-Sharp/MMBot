using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MMBot.Data;
using MMBot.Data.Entities;
using MMBot.Helpers;
using MMBot.Discord.Services.Interfaces;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace MMBot.Discord.Services
{
    public class DatabaseService : IDatabaseService
    {
        private Context _context;

        public DatabaseService(Context context) => _context = context;
        public async Task MigrateAsync() => await _context?.MigrateAsync();
        public async Task SaveDataAsync() => await _context?.SaveChangesAsync(new CancellationToken());
        public async Task<GuildSettings> LoadGuildSettingsAsync(ulong guildId) => await _context.GuildSettings.AsAsyncEnumerable().Where(x => x.GuildId == guildId).FirstOrDefaultAsync();

        
        public async Task<Clan> CreateClanAsync(ulong guildId) => (await _context.AddAsync(new Clan{ GuildId = guildId }, new CancellationToken())).Entity;
        public async Task<IList<Clan>> LoadClansAsync(ulong guildId) => await _context.Clan.AsAsyncEnumerable().Where(x => x.GuildId == guildId).ToListAsync();
        public void DeleteClan(Clan c) => _context.Remove(c);


        public async Task<Member> CreateMemberAsync(ulong guildId) => (await _context.AddAsync(new Member { GuildId = guildId }, new CancellationToken())).Entity;
        public async Task<IList<Member>> LoadMembersAsync(ulong guildId) => await _context.Member.AsAsyncEnumerable().Where(x => x.GuildId == guildId).ToListAsync();
        public void DeleteMember(Member m) => _context.Remove(m);


        public async Task<MMTimer> CreateTimerAsync(ulong guildId) => (await _context.AddAsync(new MMTimer { GuildId = guildId }, new CancellationToken())).Entity;
        public async Task<IList<MMTimer>> LoadTimerAsync(ulong? guildId = null) => await _context.Timer.AsAsyncEnumerable().Where(x => guildId is null || x.GuildId == guildId).ToListAsync();
        public async Task<MMTimer> GetTimerAsync(string name, ulong guildId) => await _context.Timer.AsAsyncEnumerable().Where(x => x.GuildId == guildId).FirstOrDefaultAsync(x => x.Name.ToLower() == name.ToLower());
        public void DeleteTimer(MMTimer t) => _context.Remove(t);

        

        public async Task<Restart> AddRestart() => (await _context.AddAsync(new Restart(), new CancellationToken())).Entity;
        public async Task<Tuple<ulong, ulong>> ConsumeRestart()
        {
            try
            {
                var r = await _context.Restart.FirstOrDefaultAsync();

                if(r != null)
                {
                    var t = new Tuple<ulong, ulong>(r.Guild, r.Channel);
                    _context.Restart.Remove(r);
                    await _context.SaveChangesAsync(new CancellationToken());
                    return t;
                }
                else
                    return default;
            }
            catch
            {
                return default;
            }
        }

        public async Task<Channel> CreateChannelAsync(ulong guildId) => (await _context.AddAsync(new Channel(), new CancellationToken())).Entity;
        public async Task<IList<Channel>> LoadChannelsAsync(ulong? guildId = null) => await _context.Channel.AsAsyncEnumerable().Where(x => guildId is null || x.GuildId == guildId).ToListAsync();
        public void DeleteChannel(Channel c) => _context.Remove(c);

        public async Task<Context> DeleteDB()
        {
            _context.Database.EnsureDeleted();
            
            var context = new Context();
            await context.MigrateAsync();
            return context;
        }

        public async Task CleanDB(IEnumerable<ulong> guildIds)
        {
            var c = await _context.Clan.ToListAsync();
            var m = await _context.Member.ToListAsync();

            // clean dead member data
            if (c is null || c.Count == 0 || m is null || m.Count == 0)
                return;

            m.Where(x => x.ClanId == 0 || x.Name == string.Empty).ForEach(x =>
            {
                var id = x.ClanId;
                _context.Remove(x);

                if (id.HasValue)
                    _context.Remove(c.FirstOrDefault(y => y.Id == id));
            });
            await _context.SaveChangesAsync();

            m.Where(x => !x.IsActive || x.ClanId == null || x.Role == Data.Enums.Role.ExMember).ForEach(x =>
            {
                x.IsActive = false;
                x.ClanId = null;
                x.Role = Data.Enums.Role.ExMember;
            });
            await _context.SaveChangesAsync();

            m.Where(x => string.IsNullOrWhiteSpace(x.Name)).ForEach(x => _context.Remove(x));
            await _context.SaveChangesAsync();

            // clean dead channel data
            if (guildIds is not null)
            {
                var ch = await LoadChannelsAsync();
                ch.Where(x => !guildIds.Contains(x.GuildId)).ForEach(cha => _context.Remove(cha));
                await _context.SaveChangesAsync();
            }
        }
    }
}
