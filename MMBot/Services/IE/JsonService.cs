using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MMBot.Data;
using MMBot.Data.Entities;
using MMBot.Services.Interfaces;
using Newtonsoft.Json;
using MMBot.Data.Interfaces;

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
                PreserveReferencesHandling = PreserveReferencesHandling.None,
                Formatting = Formatting.Indented,
                MaxDepth = 1
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
            var mmt = await _context.Timer.AsAsyncEnumerable().ToListAsync().AsTask();

            var dict = new Dictionary<string, string>
            {
                { "Member", JsonConvert.SerializeObject(am, _jsonSerializerSettings) },
                { "Clan", JsonConvert.SerializeObject(ac, _jsonSerializerSettings) },
                { "GuildSettings", JsonConvert.SerializeObject(ags, _jsonSerializerSettings) },
                { "Vacation", JsonConvert.SerializeObject(av, _jsonSerializerSettings) },
                { "Channel", JsonConvert.SerializeObject(aca, _jsonSerializerSettings) },
                { "MemberGroup", JsonConvert.SerializeObject(amg, _jsonSerializerSettings) },
                { "MMTimer", JsonConvert.SerializeObject(mmt, _jsonSerializerSettings) }
            };

            return dict;
        }

        public async Task<bool> ImportJsonToDB(IDictionary<string, string> importJson)
        {
            try
            {
                if(importJson.TryGetValue("Channel", out var channel))
                {
                    var am = JsonConvert.DeserializeObject<IList<Channel>>(channel, _jsonSerializerSettings);
                    await ImportOrUpgrade(_context.Channel, am);
                    await _context.SaveChangesAsync();
                }

                if(importJson.TryGetValue("GuildSettings", out var guild))
                {
                    var ac = JsonConvert.DeserializeObject<IList<GuildSettings>>(guild, _jsonSerializerSettings);
                    await ImportOrUpgrade(_context.GuildSettings, ac);
                    await _context.SaveChangesAsync();
                }

                if(importJson.TryGetValue("Clan", out var clan))
                {
                    var ags = JsonConvert.DeserializeObject<IList<Clan>>(clan, _jsonSerializerSettings);
                    await ImportOrUpgrade(_context.Clan, ags);
                    await _context.SaveChangesAsync();
                }

                if(importJson.TryGetValue("Member", out var member))
                {
                    var aca = JsonConvert.DeserializeObject<IList<Member>>(member, _jsonSerializerSettings);
                     await ImportOrUpgrade(_context.Member, aca);
                    await _context.SaveChangesAsync();
                }

                if(importJson.TryGetValue("Vacation", out var vac))
                {
                    var av = JsonConvert.DeserializeObject<IList<Vacation>>(vac, _jsonSerializerSettings);
                    await ImportOrUpgrade(_context.Vacation, av);
                    await _context.SaveChangesAsync();
                }

                if(importJson.TryGetValue("MemberGroup", out var mgroup))
                {
                    var amg = JsonConvert.DeserializeObject<IList<MemberGroup>>(mgroup, _jsonSerializerSettings);
                   await ImportOrUpgrade(_context.MemberGroup, amg);
                    await _context.SaveChangesAsync();
                }
                
                if(importJson.TryGetValue("MMTimer", out var timer))
                {
                    var mmt = JsonConvert.DeserializeObject<IList<MMTimer>>(timer, _jsonSerializerSettings);
                    await ImportOrUpgrade(_context.Timer, mmt);
                    await _context.SaveChangesAsync();
                }
                
                await _context.SaveChangesAsync();
                return true;
            }
            catch(Exception e)
            {
                 _logger.LogError(e.Message, e);
                return false;
            }
        }

        private async Task ImportOrUpgrade<T>(DbSet<T> currentData, IList<T> updateWithData) 
            where T : class, IHaveId
        {
            foreach(var uwd in updateWithData)
            {
                var found = await currentData.FindAsync(uwd.Id);

                if(found is not null)
                    found.Update(uwd);
                else
                    await currentData.AddAsync(uwd);
            }
        }
    }
}
