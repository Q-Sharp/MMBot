using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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