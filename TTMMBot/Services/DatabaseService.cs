using TTMMBot.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using TTMMBot.Data.Entities;

namespace TTMMBot.Services
{
    public class DatabaseService : IDatabaseService
    {
        private readonly Context _context;
        public DatabaseService(Context context)
        {
            _context = context;
        }
        public async Task Migrate() => await _context?.Database.MigrateAsync();
        public async Task SaveData() => await _context?.SaveChangesAsync();

        public Task<Member> LoadAllMemberData()
        {
            throw new System.NotImplementedException();
        }
    }
}
