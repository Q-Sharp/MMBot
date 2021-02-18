using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using MMBot.Data;
using MMBot.Data.Entities;
using MMBot.Data.Services.Interfaces;
using MMBot.Discord.Services.Interfaces;

namespace MMBot.Discord.Services
{
    public class AdminService : IAdminService
    {
        private readonly Context _context;
        private readonly IGuildSettingsService _settings;
        private readonly ICommandHandler _commandHandler;
        private readonly IDatabaseService _databaseService;
        private readonly IJsonService _jsonService;
        private readonly IHostApplicationLifetime  _hostApplicationLifetime;
        private ulong _guildId;

        private readonly string _backupDir = Path.Combine(".", "backup");
        private readonly string _import = "dataimport.zip";

        public AdminService(Context context, IGuildSettingsService settings, ICommandHandler commandHandler, IDatabaseService databaseService, IJsonService jsonService, IHostApplicationLifetime  hostApplicationLifetime)
        {
            _context = context;
            _settings = settings;
            _commandHandler = commandHandler;
            _databaseService = databaseService;
            _jsonService = jsonService;
            _hostApplicationLifetime = hostApplicationLifetime;
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

        public async Task<bool> DataImport(byte[] zipBytes)
        {
            await File.WriteAllBytesAsync(_import, zipBytes);

            var dict = await Task.Run(async () =>
            {
                Directory.CreateDirectory(_backupDir);

                ZipFile.ExtractToDirectory(_import, _backupDir);

                var dict = new Dictionary<string, string>();

                foreach (var entry in Directory.GetFiles(_backupDir))
                    dict.Add(Path.GetFileNameWithoutExtension(entry), await File.ReadAllTextAsync(entry));

                Directory.Delete(_backupDir, true);
                return dict;
            });

            var result = await _jsonService.ImportJsonToDB(dict, _context);
            File.Delete(_import);

            return result;
        }


        public void SetGuild(ulong id) 
            => _guildId = id;

        public void Truncate() 
            => _databaseService.Truncate();

        public async Task Restart(bool saveRestart = false, ulong? guildId = null, ulong? channelId = null)
        {
            if (saveRestart && guildId.HasValue && channelId.HasValue)
            {
                var r = _databaseService?.AddRestart();
                r.Guild = guildId.Value;
                r.Channel = channelId.Value;
                await _databaseService?.SaveDataAsync();
            }

            _hostApplicationLifetime?.StopApplication();
        }
    }
}
