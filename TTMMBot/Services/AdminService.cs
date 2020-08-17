using System;
using System.Collections.Generic;
using System.Linq;
using TTMMBot.Data;
using TTMMBot.Data.Entities;

namespace TTMMBot.Services
{
    public class AdminService
    {
        private Context _context { get; set; }
        private GlobalSettings _settings { get; set; }
        public AdminService(Context context, GlobalSettings settings)
        {
            _context = context;
            _settings = settings;
        }

        public void Reorder()
        {
            _context.Member.AsEnumerable()
                .OrderBy(x => x.JoinOrder)
                .ThenBy(x => x.SeasonHighest)
                .GroupBy(x => x.ClanID, (x, y) => new { Clan = x, Members = y })
                .Select(x => x.Members.ToList() as IList<Member>)
                .ToList()
                .ForEach(x =>
                {
                    var i = 1;
                    x.ToList().ForEach(m => m.JoinOrder = i++);
                });

            _context?.SaveChanges();
        }
    }
}