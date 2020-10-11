using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TTMMBot.Data;
using TTMMBot.Data.Entities;
using TTMMBot.Helpers;
using TTMMBot.Services.Interfaces;

namespace TTMMBot.Services
{
    public class DatabaseService : IDatabaseService
    {
        public Context Context { get; set; }
        public DatabaseService(Context context) => Context = context;
        
        public async Task<Clan> CreateClanAsync() => (await Context.AddAsync(new Clan(), new CancellationToken())).Entity;
        public async Task<IList<Clan>> LoadClansAsync() => await Context.Clan.ToListAsync();
        public void DeleteClan(Clan c) => Context.Remove(c);

        public async Task<Member> CreateMemberAsync() => (await Context.AddAsync(new Member(), new CancellationToken())).Entity;
        public async Task<IList<Member>> LoadMembersAsync() => await Context.Member.ToListAsync();
        public void DeleteMember(Member m) => Context.Remove(m);

        public async Task MigrateAsync() => await Context?.MigrateAsync();
        public async Task SaveDataAsync() => await Context?.SaveChangesAsync(new CancellationToken());

        public async Task<GlobalSettings> LoadGlobalSettingsAsync() => await Context.GlobalSettings.FirstOrDefaultAsync();

        public async Task<Restart> AddRestart() => (await Context.AddAsync(new Restart(), new CancellationToken())).Entity;
        public async Task<Tuple<ulong, ulong>> ConsumeRestart()
        {
            try
            {
                var r = await Context.Restart.FirstOrDefaultAsync();
                var t = new Tuple<ulong, ulong>(r.Guild, r.Channel);
                Context.Restart.Remove(r);
                await Context.SaveChangesAsync(new CancellationToken());
                return t;
            }
            catch
            {
                return null;
            }
        }

        public async Task<Channel> CreateChannelAsync() => (await Context.AddAsync(new Channel(), new CancellationToken())).Entity;
        public async Task<IList<Channel>> LoadChannelsAsync() => await Context.Channel.ToListAsync();
        public void DeleteChannel(Channel c) => Context.Remove(c);

        public async Task CleanDB()
        {
            var c = await LoadClansAsync();
            var m = await LoadMembersAsync();

            m.Where(x => x.ClanId == 0 || x.Name == string.Empty).ForEach(x =>
            {
                var id = x.ClanId;
                Context.Remove(x);

                if(id.HasValue)
                    Context.Remove(c.FirstOrDefault(y => y.ClanId == id));
                Context.SaveChanges();
            });
        }
    }
}
