using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TTMMBot.Data;
using TTMMBot.Data.Entities;
using TTMMBot.Services.Interfaces;

namespace TTMMBot.Services
{
    public class DatabaseMemberService : IDatabaseMemberService
    {
        private readonly Context _context;

        public DatabaseMemberService(Context context)
        {
            _context = context;
        }

        public async Task CreateMember()
        {

        }
    }
}
