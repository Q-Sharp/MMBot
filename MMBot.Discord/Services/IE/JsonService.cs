using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MMBot.Data;
using MMBot.Data.Entities;
using MMBot.Data.Helpers;
using MMBot.Discord.Services.Interfaces;

namespace MMBot.Discord.Services.IE;

public class JsonService : IJsonService
{
    private readonly Context _context;
    private readonly ILogger<JsonService> _logger;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public JsonService(Context context, ILogger<JsonService> logger)
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
        var dict = new Dictionary<string, string>
            {
                { "Member", JsonSerializer.Serialize(await _context.Member.AsAsyncEnumerable().ToListAsync(), _jsonSerializerOptions) },
                { "Clan", JsonSerializer.Serialize(await _context.Clan.AsAsyncEnumerable().ToListAsync().AsTask(), _jsonSerializerOptions) },
                { "GuildSettings", JsonSerializer.Serialize(await _context.GuildSettings.AsAsyncEnumerable().ToListAsync().AsTask(), _jsonSerializerOptions) },
                { "Vacation", JsonSerializer.Serialize(await _context.Vacation.AsAsyncEnumerable().ToListAsync().AsTask(), _jsonSerializerOptions) },
                { "Channel", JsonSerializer.Serialize(await _context.Channel.AsAsyncEnumerable().ToListAsync().AsTask(), _jsonSerializerOptions) },
                { "MemberGroup", JsonSerializer.Serialize(await _context.MemberGroup.AsAsyncEnumerable().ToListAsync().AsTask(), _jsonSerializerOptions) },
                { "Season", JsonSerializer.Serialize(await _context.Season.AsAsyncEnumerable().ToListAsync().AsTask(), _jsonSerializerOptions) },
                { "Strike", JsonSerializer.Serialize(await _context.Strike.AsAsyncEnumerable().ToListAsync().AsTask(), _jsonSerializerOptions) },
                { "RaidBoss", JsonSerializer.Serialize(await _context.RaidBoss.AsAsyncEnumerable().ToListAsync().AsTask(), _jsonSerializerOptions) },
                { "RaidParticipation", JsonSerializer.Serialize(await _context.RaidParticipation.AsAsyncEnumerable().ToListAsync().AsTask(), _jsonSerializerOptions) },
                { "MMTimer", JsonSerializer.Serialize(await _context.Timer.AsAsyncEnumerable().ToListAsync().AsTask(), _jsonSerializerOptions) },
                { "MemberRoom", JsonSerializer.Serialize(await _context.MemberRoom.AsAsyncEnumerable().ToListAsync().AsTask(), _jsonSerializerOptions) },
            };

        return dict;
    }

    public async Task<bool> ImportJsonToDB(IDictionary<string, string> importJson, Context context = null)
    {
        try
        {
            if (importJson.TryGetValue("Channel", out var channel))
            {
                var am = JsonSerializer.Deserialize<IList<Channel>>(channel, _jsonSerializerOptions);
                await context.Channel.ImportOrUpgrade(am);
                await context.SaveChangesAsync();
            }

            if (importJson.TryGetValue("GuildSettings", out var guild))
            {
                var ac = JsonSerializer.Deserialize<IList<Data.Entities.GuildSettings>>(guild, _jsonSerializerOptions);
                await context.GuildSettings.ImportOrUpgrade(ac);
                await context.SaveChangesAsync();
            }

            if (importJson.TryGetValue("Clan", out var clan))
            {
                var ags = JsonSerializer.Deserialize<IList<Clan>>(clan, _jsonSerializerOptions);
                await context.Clan.ImportOrUpgrade(ags);
                await context.SaveChangesAsync();
            }

            if (importJson.TryGetValue("Season", out var season))
            {
                var s = JsonSerializer.Deserialize<IList<Season>>(season, _jsonSerializerOptions);
                await context.Season.ImportOrUpgrade(s);
                await context.SaveChangesAsync();
            }

            if (importJson.TryGetValue("Vacation", out var vac))
            {
                var av = JsonSerializer.Deserialize<IList<Vacation>>(vac, _jsonSerializerOptions);
                await context.Vacation.ImportOrUpgrade(av);
                await context.SaveChangesAsync();
            }

            if (importJson.TryGetValue("MemberGroup", out var mgroup))
            {
                var amg = JsonSerializer.Deserialize<IList<MemberGroup>>(mgroup, _jsonSerializerOptions);
                await context.MemberGroup.ImportOrUpgrade(amg);
                await context.SaveChangesAsync();
            }

            if (importJson.TryGetValue("Member", out var member))
            {
                var aca = JsonSerializer.Deserialize<IList<Member>>(member, _jsonSerializerOptions);
                await context.Member.ImportOrUpgrade(aca);
                await context.SaveChangesAsync();
            }

            if (importJson.TryGetValue("RaidBoss", out var raidBoss))
            {
                var rb = JsonSerializer.Deserialize<IList<RaidBoss>>(raidBoss, _jsonSerializerOptions);
                await context.RaidBoss.ImportOrUpgrade(rb);
                await context.SaveChangesAsync();
            }

            if (importJson.TryGetValue("RaidParticipation", out var raidParticipation))
            {
                var rp = JsonSerializer.Deserialize<IList<RaidParticipation>>(raidParticipation, _jsonSerializerOptions);
                await context.RaidParticipation.ImportOrUpgrade(rp);
                await context.SaveChangesAsync();
            }

            if (importJson.TryGetValue("MMTimer", out var timer))
            {
                var mmt = JsonSerializer.Deserialize<IList<MMTimer>>(timer, _jsonSerializerOptions);
                await context.Timer.ImportOrUpgrade(mmt);
                await context.SaveChangesAsync();
            }

            if (importJson.TryGetValue("MemberRoom", out var mRoom))
            {
                var mr = JsonSerializer.Deserialize<IList<MemberRoom>>(mRoom, _jsonSerializerOptions);
                await context.MemberRoom.ImportOrUpgrade(mr);
                await context.SaveChangesAsync();
            }

            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message, e);
            return false;
        }
    }
}
