using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TTMMBot.Data;
using TTMMBot.Data.Entities;
using TTMMBot.Services.Interfaces;

namespace TTMMBot.Services
{
    public class AdminService : IAdminService
    {
        private Context _context;
        private IGuildSettingsService _settings;
        private ICommandHandler _commandHandler;

        public AdminService(Context context, IGuildSettingsService settings, ICommandHandler commandHandler)
        {
            _context = context;
            _settings = settings;
            _commandHandler = commandHandler;
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

        public async Task Reorder()
        {
            _context.Member.AsEnumerable()
                .OrderBy(x => x.Join, JoinComparer.Create(_settings.ClanSize))
                .ThenBy(x => x.SHigh)
                .GroupBy(x => x.ClanId, (x, y) => new { Clan = x, Members = y })
                .Select(x => x.Members.ToList() as IList<Member>)
                .ToList()
                .ForEach(x =>
                {
                    var i = 1;
                    x.ToList().ForEach(m => m.Join = i++);
                });

            await _context?.SaveChangesAsync();
        }
    }
}
