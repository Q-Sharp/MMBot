using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
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
        private ulong _guildId;

        private readonly string _backupDir = Path.Combine(".", "backup");
        //private readonly string _export = "dataexport.zip";
        private readonly string _import = "dataimport.zip";

        public AdminService(Context context, IGuildSettingsService settings, ICommandHandler commandHandler, IDatabaseService databaseService, IJsonService jsonService)
        {
            _context = context;
            _settings = settings;
            _commandHandler = commandHandler;
            _databaseService = databaseService;
            _jsonService = jsonService;
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

        public async Task InitDataImport(ulong guildId, ulong channelId)
            => await Restart(true, guildId, channelId, true);

        public async Task<bool> FinishDataImport()
        {
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

        public void DeleteDb() 
            => _databaseService.DeleteDB();

        public async Task Restart(bool saveRestart = false, ulong? guildId = null, ulong? channelId = null, bool isDataImport = false)
        {
            if (saveRestart && guildId.HasValue && channelId.HasValue)
            {
                var r = await _databaseService?.AddRestart();
                r.Guild = guildId.Value;
                r.Channel = channelId.Value;
                r.DBImport = isDataImport;
                await _databaseService?.SaveDataAsync();
            }

            Process.Start(AppDomain.CurrentDomain.FriendlyName);
            Environment.Exit(0);
        }
    }
}
