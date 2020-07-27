using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TTMMBot.Data;
using TTMMBot.Data.Entities;
using TTMMBot.Services.Interfaces;

namespace TTMMBot.Services
{
    public class DatabaseClanService : IDatabaseClanService
    {
        private readonly Context _context;

        public DatabaseClanService(Context context) => _context = context;

        public async Task CreateClanAsync(Clan clan) => await _context.AddAsync(clan);

        public async Task<Clan> FindClanAsync(Clan clan) => await _context.Clan.FirstOrDefaultAsync(c => c.ClanID == clan.ClanID || c.Tag == clan.Tag || c.Name == clan.Name);

        public async Task DeleteClanAsync(Clan clan)
        {
            var c = await _context.Clan.FirstOrDefaultAsync(c => c.ClanID == clan.ClanID || c.Tag == clan.Tag || c.Name == clan.Name);
            _context.Remove(c);
        }

        public async Task<IList<Clan>> LoadClansAsync() => await _context.Clan.ToListAsync();
    }
}
