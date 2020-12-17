using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MMBot.Data;
using MMBot.Data.Entities;
using MMBot.Discord.Services.Interfaces;

namespace MMBot.Discord.Services
{
    public class AdminService : IAdminService
    {
        private Context _context;
        private IGuildSettingsService _settings;
        private ICommandHandler _commandHandler;
        private IDatabaseService _databaseService;
        private ulong _guildId;

        public AdminService(Context context, IGuildSettingsService settings, ICommandHandler commandHandler, IDatabaseService databaseService)
        {
            _context = context;
            _settings = settings;
            _commandHandler = commandHandler;
            _databaseService = databaseService;
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

        public async Task Reorder(ulong guildId)
        {
            var settings = await _settings.GetGuildSettingsAsync(guildId);
            (await _context.Member.AsAsyncEnumerable().Where(x => x.GuildId == _guildId).ToListAsync())
                .OrderBy(x => x.Join, JoinComparer.Create(settings.ClanSize))
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


        public void SetGuild(ulong id) => _guildId = id;

        public async Task<Context> DeleteDb() => await _databaseService.DeleteDB();

        public Task Restart()
        {
            Process.Start(AppDomain.CurrentDomain.FriendlyName);
            return Task.Run(async () => 
            {
                await Task.Delay(100);
                Environment.Exit(0);
            });
        }
    }
}
