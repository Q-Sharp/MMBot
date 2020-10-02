using System.Collections.Generic;
using System.Linq;
using TTMMBot.Data;
using TTMMBot.Data.Entities;
using TTMMBot.Services.Interfaces;

namespace TTMMBot.Services
{
    public class AdminService : IAdminService
    {
        public Context Context { get; set; }
        public IGlobalSettingsService Settings { get; set; }
        public ICommandHandler CommandHandler { get; set; }

        public AdminService(Context context, IGlobalSettingsService settings, ICommandHandler commandHandler)
        {
            Context = context;
            Settings = settings;
            CommandHandler = commandHandler;
        }

        public class JoinComparer : IComparer<int>
        {
            private readonly int _maxSize;

            public JoinComparer(int maxSize) => _maxSize = maxSize;
            public static JoinComparer Create(int maxSize) => new JoinComparer(maxSize);

            public int Compare(int x, int y)
            {
                var nx = x;
                var ny = y;

                if (x == 0)
                    nx = _maxSize + 1;

                if (y == 0)
                    ny = _maxSize + 1;

                return nx - ny;
            }
        }

        public void Reorder()
        {
            Context.Member.AsEnumerable()
                .OrderBy(x => x.Join, JoinComparer.Create(Settings.ClanSize))
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
