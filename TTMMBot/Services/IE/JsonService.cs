using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Json;
using TTMMBot.Data;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using TTMMBot.Services.Interfaces;
using Newtonsoft.Json;
using TTMMBot.Data.Entities;

namespace TTMMBot.Services.IE
{
    public class JsonService : IJsonService
    {
        private Context _context;
        private ILogger<CsvService> _logger;

        private JsonSerializerSettings _jsonSerializerSettings;

        public JsonService(Context context, ILogger<CsvService> logger)
        {
            _context = context;
            _logger = logger;

            _jsonSerializerSettings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                ObjectCreationHandling = ObjectCreationHandling.Auto,
                Formatting = Formatting.Indented
            };
        }

        public async Task<IDictionary<string, string>> ExportDBToJson()
        {
            var am = await _context.Member.AsAsyncEnumerable().ToListAsync().AsTask();
            var ac = await _context.Clan.AsAsyncEnumerable().ToListAsync().AsTask();
            var ags = await _context.GuildSettings.AsAsyncEnumerable().ToListAsync().AsTask();
            var av = await _context.Vacation.AsAsyncEnumerable().ToListAsync().AsTask();
            var aca = await _context.Channel.AsAsyncEnumerable().ToListAsync().AsTask();
            var amg = await _context.MemberGroup.AsAsyncEnumerable().ToListAsync().AsTask();

            var dict = new Dictionary<string, string>
            {
                { "Member", JsonConvert.SerializeObject(am, _jsonSerializerSettings) },
                { "Clan", JsonConvert.SerializeObject(ac, _jsonSerializerSettings) },
                { "GuildSettings", JsonConvert.SerializeObject(ags, _jsonSerializerSettings) },
                { "Vacation", JsonConvert.SerializeObject(av, _jsonSerializerSettings) },
                { "Channel", JsonConvert.SerializeObject(aca, _jsonSerializerSettings) },
                { "MemberGroups", JsonConvert.SerializeObject(amg, _jsonSerializerSettings) }
            };

            return dict;
        }

        public async Task<bool> ImportJsonToDB(IDictionary<string, string> importJson)
        {
            IList<Channel> am = JsonConvert.DeserializeObject<IList<Channel>>(importJson["Channel"], _jsonSerializerSettings);
            IList<GuildSettings> ac = JsonConvert.DeserializeObject<IList<GuildSettings>>(importJson["GuildSettings"], _jsonSerializerSettings);
            IList<Clan> ags= JsonConvert.DeserializeObject<IList<Clan>>(importJson["Clan"], _jsonSerializerSettings);
            IList<Vacation> av = JsonConvert.DeserializeObject<IList<Vacation>>(importJson["Vacation"], _jsonSerializerSettings);
            IList<Member> aca = JsonConvert.DeserializeObject<IList<Member>>(importJson["Member"], _jsonSerializerSettings);
            IList<MemberGroup> amg = JsonConvert.DeserializeObject<IList<MemberGroup>>(importJson["MemberGroup"], _jsonSerializerSettings);


            return await Task.Run(() => false);
        }
    }
}
