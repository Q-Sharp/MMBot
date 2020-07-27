using TTMMBot.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using TTMMBot.Data.Entities;
using TTMMBot.Services.Interfaces;

namespace TTMMBot.Services
{
    public class DatabaseService : IDatabaseService
    {
        private readonly Context _context;

        public IDatabaseClanService ClanService { get; set; }
        public IDatabaseMemberService MemberService { get; set; }

        public DatabaseService(Context context, IDatabaseClanService clanService, IDatabaseMemberService memberService)
        {
            _context = context;
            ClanService = clanService;
            MemberService = memberService;
        }

        public async Task Migrate() => await _context?.Database.MigrateAsync();
        public async Task SaveDataAsync() => await _context?.SaveChangesAsync();
    }
}
