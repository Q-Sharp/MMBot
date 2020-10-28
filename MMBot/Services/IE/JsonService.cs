using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MMBot.Data;
using MMBot.Data.Entities;
using MMBot.Services.Interfaces;

namespace MMBot.Services.IE
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
                { "MemberGroup", JsonConvert.SerializeObject(amg, _jsonSerializerSettings) }
            };

            return dict;
        }

        public async Task<bool> ImportJsonToDB(IDictionary<string, string> importJson)
        {
            try
            {
                var am = JsonConvert.DeserializeObject<IList<Channel>>(importJson["Channel"], _jsonSerializerSettings);
                var ac = JsonConvert.DeserializeObject<IList<GuildSettings>>(importJson["GuildSettings"], _jsonSerializerSettings);
                var ags = JsonConvert.DeserializeObject<IList<Clan>>(importJson["Clan"], _jsonSerializerSettings);
                var aca = JsonConvert.DeserializeObject<IList<Member>>(importJson["Member"], _jsonSerializerSettings);
                var av = JsonConvert.DeserializeObject<IList<Vacation>>(importJson["Vacation"], _jsonSerializerSettings);
                var amg = JsonConvert.DeserializeObject<IList<MemberGroup>>(importJson["MemberGroup"], _jsonSerializerSettings);

                await ImportOrUpgrade(_context.Channel, am);
                await ImportOrUpgrade(_context.GuildSettings, ac);
                await ImportOrUpgrade(_context.Clan, ags);
                await ImportOrUpgrade(_context.Member, aca);
                await ImportOrUpgrade(_context.Vacation, av);
                await ImportOrUpgrade(_context.MemberGroup, amg);

                await _context.SaveChangesAsync();
                return true;
            }
            catch(Exception e)
            {
                 _logger.LogError(e.Message);
                return false;
            }
        }

        private async Task ImportOrUpgrade<T>(DbSet<T> currentData, IList<T> updateWithData) 
            where T : class, IHaveId
        {
            foreach(var uwd in updateWithData)
            {
                var found = await currentData.FindAsync(uwd.Id);

                if(found != null)
                    found.Update(uwd);
                else
                    await currentData.AddAsync(uwd);
            }
        }
    }
}
