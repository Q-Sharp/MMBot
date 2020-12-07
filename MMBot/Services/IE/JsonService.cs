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
                MaxDepth = 1,
                WriteIndented = true,
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

            var json = System.Text.Json.JsonSerializer.Serialize(am, _jsonSerializerOptions);

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

        public async Task<bool> ImportJsonToDB(IDictionary<string, string> importJson)
        {
            try
            {
                if(importJson.TryGetValue("Channel", out var channel))
                {
                    var am = JsonSerializer.Deserialize<IList<Channel>>(channel, _jsonSerializerOptions);
                    await ImportOrUpgrade(_context.Channel, am);
                    await _context.SaveChangesAsync();
                }

                if(importJson.TryGetValue("GuildSettings", out var guild))
                {
                    var ac = JsonSerializer.Deserialize<IList<GuildSettings>>(guild, _jsonSerializerOptions);
                    await ImportOrUpgrade(_context.GuildSettings, ac);
                    await _context.SaveChangesAsync();
                }

                if(importJson.TryGetValue("Clan", out var clan))
                {
                    var ags = JsonSerializer.Deserialize<IList<Clan>>(clan, _jsonSerializerOptions);
                    await ImportOrUpgrade(_context.Clan, ags);
                    await _context.SaveChangesAsync();
                }

                if(importJson.TryGetValue("Member", out var member))
                {
                    var aca = JsonSerializer.Deserialize<IList<Member>>(member, _jsonSerializerOptions);
                    await ImportOrUpgrade(_context.Member, aca);
                    await _context.SaveChangesAsync();
                }

                if(importJson.TryGetValue("Vacation", out var vac))
                {
                    var av = JsonSerializer.Deserialize<IList<Vacation>>(vac, _jsonSerializerOptions);
                    await ImportOrUpgrade(_context.Vacation, av);
                    await _context.SaveChangesAsync();
                }

                if(importJson.TryGetValue("MemberGroup", out var mgroup))
                {
                    var amg = JsonSerializer.Deserialize<IList<MemberGroup>>(mgroup, _jsonSerializerOptions);
                    await ImportOrUpgrade(_context.MemberGroup, amg);
                    await _context.SaveChangesAsync();
                }
                
                if(importJson.TryGetValue("MMTimer", out var timer))
                {
                    var mmt = JsonSerializer.Deserialize<IList<MMTimer>>(timer, _jsonSerializerOptions);
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

        private async static Task ImportOrUpgrade<T>(DbSet<T> currentData, IList<T> updateWithData) 
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
