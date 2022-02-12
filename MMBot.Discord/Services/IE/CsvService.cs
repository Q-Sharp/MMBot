using System.Data;
using System.Globalization;
using System.Text;
using CsvHelper;
using Microsoft.Extensions.Logging;
using MMBot.Data;
using MMBot.Data.Entities;
using MMBot.Data.Enums;
using MMBot.Data.Helpers;
using MMBot.Discord.Services.GuildSettings;
using MMBot.Discord.Services.Interfaces;

namespace MMBot.Discord.Services.IE;

public class CsvService : ICsvService
{
    private readonly Context _context;
    private readonly GuildSettingsService _settings;
    private readonly ILogger<CsvService> _logger;

    public CsvService(Context context, GuildSettingsService settings, ILogger<CsvService> logger)
    {
        _context = context;
        _settings = settings;
        _logger = logger;
    }

    public async Task<Exception> ImportCsv(byte[] csv, ulong guildId)
    {
        using var mem = new MemoryStream(csv);
        using var reader = new StreamReader(mem, Encoding.UTF8);
        using var csvReader = new CsvReader(reader, CultureInfo.CurrentCulture);

        //csvReader.Configuration.HeaderValidated = null;
        //csvReader.Configuration.MissingFieldFound = null;
        //csvReader.Configuration.BadDataFound = null;
        //csvReader.Configuration.Delimiter = ",";

        using var dr = new CsvDataReader(csvReader);
        var dt = new DataTable();
        dt.Load(dr);

        try
        {
            if (dt.Columns.Contains("IGN"))
                dt.Columns["IGN"].ColumnName = "Name";

            if (dt.Columns.Contains("Discord Status"))
                dt.Columns["Discord Status"].ColumnName = "DiscordStatus";

            if (dt.Columns.Contains("Clan"))
                dt.Columns["Clan"].ColumnName = "ClanTag";

            if (dt.Columns.Contains("AT-highest"))
                dt.Columns["AT-highest"].ColumnName = "AHigh";

            if (dt.Columns.Contains("S-highest"))
                dt.Columns["S-highest"].ColumnName = "SHigh";

            if (dt.Columns.Contains("Join Date"))
                dt.Columns["Join Date"].ColumnName = "Join";
        }
        catch
        {
            // ignore
        }

        try
        {
            var clanTags = dt.AsEnumerable().OrderByDescending(x => x.Field<string>("SHigh"))
                .Select(x => x.Field<string>("ClanTag"))
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToList();

            var c = _context.Clan.AsQueryable().Where(x => x.GuildId == guildId).ToList();
            if (c.Any(x => !clanTags.Contains(x.Tag)))
            {
                var i = c.Any() ? c.Select(x => x.SortOrder).Max() + 1 : 1;

                foreach (var tag in clanTags)
                {
                    var clan = new Clan
                    {
                        Tag = tag,
                        Name = getName(tag),
                        SortOrder = i++,
                        GuildId = guildId
                    };

                    await _context.Clan.ImportOrUpgradeWithIdentifier(clan, guildId);
                    await _context.SaveChangesAsync();
                }
            }
        }
        catch
        {
            // ignore e
        }

        try
        {
            foreach (DataRow row in dt.Rows)
            {
                if (!row.Table.Columns.Contains("Name") || row["Name"] == DBNull.Value || string.IsNullOrWhiteSpace((string)row["Name"]))
                    continue;

                var me = row["Name"] != DBNull.Value && _context.Member.Any() ? _context.Member.FirstOrDefault(
                    x => x.Name.ToLower() == ((string)row["Name"]).ToLower() && x.GuildId == guildId) : null;

                if (me is null)
                {
                    me = new Member();
                    await _context.Member.AddAsync(me);
                }

                me.IsActive = true;

                if (row.Table.Columns.Contains("Name") && row["Name"] != DBNull.Value)
                    me.Name = (string)row["Name"];

                if (row.Table.Columns.Contains("ClanTag") && row["ClanTag"] != DBNull.Value && !string.IsNullOrEmpty((string)row["ClanTag"]))
                {
                    var tag = (string)row["ClanTag"];
                    string clanName = null;

                    if (row.Table.Columns.Contains("ClanName") && row["ClanName"] != DBNull.Value && !string.IsNullOrEmpty((string)row["ClanName"]))
                        clanName = (string)row["ClanName"];

                    me.Clan = _context.Clan.FirstOrDefault(x => x.Tag == tag && (clanName == null || x.Name == clanName));
                    me.ClanId = me.Clan?.Id;
                }

                if (row.Table.Columns.Contains("Discord") && row["Discord"] != DBNull.Value)
                    me.Discord = (string)row["Discord"];

                if (row.Table.Columns.Contains("Role") && row["Role"] != DBNull.Value && Enum.TryParse(typeof(Role), ((string)row["Role"]).Replace("-", ""), out var er))
                    me.Role = (Role)er;

                if (row.Table.Columns.Contains("AHigh") && row["AHigh"] != DBNull.Value && int.TryParse((string)row["AHigh"], out var ath))
                    me.AHigh = ath;

                //if (row.Table.Columns.Contains("SHigh") && row["SHigh"] != DBNull.Value && int.TryParse((string)row["SHigh"], out var sh))
                //    me.SHigh = sh;

                //if (row.Table.Columns.Contains("Donations") && row["Donations"] != DBNull.Value && int.TryParse((string)row["Donations"], out var d))
                //    me.Donations = d;

                if (row.Table.Columns.Contains("Join") && row["Join"] != DBNull.Value && int.TryParse((string)row["Join"], out var jd))
                    me.Join = jd;

                if (row.Table.Columns.Contains("Current") && row["Current"] != DBNull.Value && int.TryParse((string)row["Current"], out var cu))
                    me.Current = cu;

                if (row.Table.Columns.Contains("DiscordStatus") && row["DiscordStatus"] != DBNull.Value
                    && Enum.TryParse(typeof(DiscordStatus), ((string)row["DiscordStatus"]).Replace("No idea", "NoIdea").Replace(" ", ""), out var ds))
                    me.DiscordStatus = (DiscordStatus)ds;

                if (row.Table.Columns.Contains("IgnoreOnMoveUp") && row["IgnoreOnMoveUp"] != DBNull.Value && bool.TryParse((string)row["IgnoreOnMoveUp"], out var iomu))
                    me.IgnoreOnMoveUp = iomu;

                if (me.Role == Role.ExMember)
                {
                    me.ClanId = null;
                    me.IsActive = false;
                }

                me.LastUpdated = DateTime.UtcNow;
                me.GuildId = guildId;

                await _context.SaveChangesAsync();
            }
        }
        catch (Exception e)
        {
            return e;
        }

        return default;
    }

    private string getName(string tag)
    {
        switch (tag)
        {
            case "TT":
                return "The Tavern";
            case "TT2":
                return "The Tavern 2";
            case "TT3":
                return "The Tavern 3";
            default:
                return tag;
        }
    }

    public async Task<byte[]> ExportCsv(ulong guildID)
    {
        await using var mem = new MemoryStream();
        await using var writer = new StreamWriter(mem, Encoding.UTF8);
        await using var csvWriter = new CsvWriter(writer, CultureInfo.CurrentCulture);

        //csvWriter.Configuration.Delimiter = ",";

        var m = _context.Member.AsEnumerable().Where(x => x.GuildId == guildID).ToList();
        await csvWriter.WriteRecordsAsync(m.ToArray().AsEnumerable());

        return mem.ToArray();
    }
}
