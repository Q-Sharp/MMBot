using System.Collections.Generic;
using System.Linq;
using TTMMBot.Data;
using TTMMBot.Data.Entities;

namespace TTMMBot.Services
{
    public class AdminService : IAdminService
    {
        public Context Context { get; set; }
        public GlobalSettingsService Settings { get; set; }
        public AdminService(Context context, GlobalSettingsService settings)
        {
            Context = context;
            Settings = settings;
        }

        public class JoinComparer : IComparer<int>
        {
            public static JoinComparer Create() => new JoinComparer();

            public int Compare(int x, int y)
            {
                var nx = x;
                var ny = y;

                if (x == 0)
                    nx = 21;

                if (y == 0)
                    ny = 21;

                return nx - ny;
            }
        }

        public void Reorder()
        {
            Context.Member.AsEnumerable()
                .OrderBy(x => x.Join, JoinComparer.Create())
                .ThenBy(x => x.SHigh)
                .GroupBy(x => x.ClanId, (x, y) => new { Clan = x, Members = y })
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