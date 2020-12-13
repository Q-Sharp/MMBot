using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MMBot.Data;
using MMBot.Data.Entities;
using MMBot.Services.Interfaces;
using MMBot.Data.Interfaces;
using System.Text.Json.Serialization;
using MMBot.Data.Helpers;

namespace MMBot.Services.IE
{
    public class JsonService : IJsonService
    {
        private readonly Context _context;
        private readonly ILogger<CsvService> _logger;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public JsonService(Context context, ILogger<CsvService> logger)
        {
            _context = context;
            _logger = logger;

            _jsonSerializerOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                IgnoreReadOnlyProperties = true,
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
                { "Member", JsonSerializer.Serialize(am, _jsonSerializerOptions) },
                { "Clan", JsonSerializer.Serialize(ac, _jsonSerializerOptions) },
                { "GuildSettings", JsonSerializer.Serialize(ags, _jsonSerializerOptions) },
                { "Vacation", JsonSerializer.Serialize(av, _jsonSerializerOptions) },
                { "Channel", JsonSerializer.Serialize(aca, _jsonSerializerOptions) },
                { "MemberGroup", JsonSerializer.Serialize(amg, _jsonSerializerOptions) },
                { "MMTimer", JsonSerializer.Serialize(mmt, _jsonSerializerOptions) }
            };

            return dict;
        }

        public async Task<bool> ImportJsonToDB(IDictionary<string, string> importJson, Context context = null)
        {
            try
            {
                if(importJson.TryGetValue("Channel", out var channel))
                {
                    var am = JsonSerializer.Deserialize<IList<Channel>>(channel, _jsonSerializerOptions);
                    await context.Channel.ImportOrUpgrade(am);
                    await context.SaveChangesAsync();
                }

                if(importJson.TryGetValue("GuildSettings", out var guild))
                {
                    var ac = JsonSerializer.Deserialize<IList<GuildSettings>>(guild, _jsonSerializerOptions);
                    await context.GuildSettings.ImportOrUpgrade(ac);
                    await context.SaveChangesAsync();
                }

                if(importJson.TryGetValue("Clan", out var clan))
                {
                    var ags = JsonSerializer.Deserialize<IList<Clan>>(clan, _jsonSerializerOptions);
                    await context.Clan.ImportOrUpgrade(ags);
                    await context.SaveChangesAsync();
                }

                if(importJson.TryGetValue("Vacation", out var vac))
                {
                    var av = JsonSerializer.Deserialize<IList<Vacation>>(vac, _jsonSerializerOptions);
                    await context.Vacation.ImportOrUpgrade(av);
                    await context.SaveChangesAsync();
                }

                if(importJson.TryGetValue("MemberGroup", out var mgroup))
                {
                    var amg = JsonSerializer.Deserialize<IList<MemberGroup>>(mgroup, _jsonSerializerOptions);
                    await context.MemberGroup.ImportOrUpgrade(amg);
                    await context.SaveChangesAsync();
                }

                if(importJson.TryGetValue("Member", out var member))
                {
                    var aca = JsonSerializer.Deserialize<IList<Member>>(member, _jsonSerializerOptions);
                    await context.Member.ImportOrUpgrade(aca);
                    await context.SaveChangesAsync();
                }
                
                if(importJson.TryGetValue("MMTimer", out var timer))
                {
                    var mmt = JsonSerializer.Deserialize<IList<MMTimer>>(timer, _jsonSerializerOptions);
                    await context.Timer.ImportOrUpgrade(mmt);
                    await context.SaveChangesAsync();
                }
                
                await context.SaveChangesAsync();
                return true;
            }
            catch(Exception e)
            {
                 _logger.LogError(e.Message, e);
                return false;
            }
        }
    }
}
