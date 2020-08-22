using System.Collections.Generic;
using System.Linq;
using TTMMBot.Data;
using TTMMBot.Data.Entities;

namespace TTMMBot.Services
{
    public class AdminService
    {
        public Context Context { get; set; }
        public GlobalSettings Settings { get; set; }
        public AdminService(Context context, GlobalSettings settings)
        {
            Context = context;
            Settings = settings;
        }

        public void Reorder()
        {
            Context.Member.AsEnumerable()
                .OrderBy(x => x.Join)
                .ThenBy(x => x.SHigh)
                .GroupBy(x => x.ClanID, (x, y) => new { Clan = x, Members = y })
                .Select(x => x.Members.ToList() as IList<Member>)
                .ToList()
                .ForEach(x =>
                {
                    var i = 1;
                    x.ToList().ForEach(m => m.Join = i++);
                });

            Context?.SaveChanges();
        }
    }
}