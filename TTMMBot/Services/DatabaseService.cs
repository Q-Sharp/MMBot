using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TTMMBot.Data;
using TTMMBot.Data.Entities;

namespace TTMMBot.Services
{
    public class DatabaseService : IDatabaseService
    {
        public Context Context { get; set; }
        public DatabaseService(Context context) => Context = context;
        
        public async Task<Clan> CreateClanAsync() => (await Context.AddAsync(new Clan())).Entity;

        public async Task<Member> CreateMemberAsync() => (await Context.AddAsync(new Member())).Entity;

        public async Task<IList<Clan>> LoadClansAsync() => await Context.Clan.ToListAsync();

        public async Task<IList<Member>> LoadMembersAsync() => await Context.Member.ToListAsync();

        public void DeleteClan(Clan c) => Context.Remove(c);

        public void DeleteMember(Member m) => Context.Remove(m);

        public async Task MigrateAsync() => await Context?.Database.MigrateAsync();
        public async Task SaveDataAsync() => await Context?.SaveChangesAsync();
    }
}
