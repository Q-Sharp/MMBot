using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.Extensions.Logging;
using MMBot.Data;
using MMBot.Data.Entities;
using MMBot.Data.Enums;
using MMBot.Services.Interfaces;

namespace MMBot.Services.IE
{
    public class CsvService : ICsvService
    {
        private Context _context;
        private GuildSettingsService _settings;
        private ILogger<CsvService> _logger;

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

            csvReader.Configuration.HeaderValidated = null;
            csvReader.Configuration.MissingFieldFound = null;
            csvReader.Configuration.BadDataFound = null;
            csvReader.Configuration.Delimiter = ",";

             using var dr = new CsvDataReader(csvReader);
             var dt = new DataTable();
             dt.Load(dr);

            try
            {
                if(dt.Columns.Contains("IGN"))
                    dt.Columns["IGN"].ColumnName = "Name";

                if(dt.Columns.Contains("Discord Status"))
                    dt.Columns["Discord Status"].ColumnName = "DiscordStatus";

                if(dt.Columns.Contains("Clan"))
                    dt.Columns["Clan"].ColumnName = "ClanTag";

                if(dt.Columns.Contains("AT-highest"))
                    dt.Columns["AT-highest"].ColumnName = "AHigh";

                if(dt.Columns.Contains("S-highest"))
                    dt.Columns["S-highest"].ColumnName = "SHigh";

                if(dt.Columns.Contains("Join Date"))
                    dt.Columns["Join Date"].ColumnName = "Join";
            }
            catch
            {
                // ignore
            }

            try
            {
                var clanTags = dt.Rows.AsQueryable().Cast<object>().Where(x => x != DBNull.Value).OfType<string>().Distinct().ToList();

                if (!_context.Clan.Any(x => !clanTags.Contains(x.Tag)))
                {
                    foreach(var tag in clanTags)
                    {
                        var clan = await _context.Clan.FirstOrDefaultAsync(c => c.Tag == tag) ?? new Clan();
                        clan.Tag = tag;
                        clan.Name = getName(tag);
                        clan.SortOrder = _context.Clan.AsQueryable().Select(x => x.SortOrder).Max() + 1;
                        clan.GuildId = guildId;
                    }
                    await _context.SaveChangesAsync();
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
                    var me = row["Name"] != DBNull.Value && _context.Member.Any() ? _context.Member.FirstOrDefault(
                        x => x.Name.ToLower() == ((string)row["Name"]).ToLower()) : null;

                    if(me is null)
                    {
                        me = new Member();
                        await _context.Member.AddAsync(me);
                    }

                    me.IsActive = true;

                    if (row.Table.Columns.Contains("Name") && row["Name"] != DBNull.Value)
                        me.Name = (string)row["Name"];

                    if (row.Table.Columns.Contains("ClanTag") && row["ClanTag"] != DBNull.Value && !string.IsNullOrEmpty((string)row["ClanTag"]))
                    {
                        me.Clan = _context.Clan.FirstOrDefault(x => x.Tag == (string)row["ClanTag"]);
                        me.ClanId =  me.Clan?.Id;
                    }

                    if (row.Table.Columns.Contains("Discord") && row["Discord"] != DBNull.Value)
                        me.Discord = (string)row["Discord"];

                    if (row.Table.Columns.Contains("Role") && row["Role"] != DBNull.Value && Enum.TryParse(typeof(Role), ((string)row["Role"]).Replace("-", ""), out var er))
                        me.Role = (Role)er;

                    if (row.Table.Columns.Contains("AHigh") && row["AHigh"] != DBNull.Value && int.TryParse((string)row["AHigh"], out var ath))
                        me.AHigh = ath;

                    if (row.Table.Columns.Contains("SHigh") && row["SHigh"] != DBNull.Value && int.TryParse((string)row["SHigh"], out var sh))
                        me.SHigh = sh;

                    if (row.Table.Columns.Contains("Donations") && row["Donations"] != DBNull.Value && int.TryParse((string)row["Donations"], out var d))
                        me.Donations = d;

                    if (row.Table.Columns.Contains("Join") && row["Join"] != DBNull.Value && int.TryParse((string)row["Join"], out var jd))
                        me.Join = jd;

                    if (row.Table.Columns.Contains("DiscordStatus") && row["DiscordStatus"] != DBNull.Value && Enum.TryParse(typeof(DiscordStatus), ((string)row["DiscordStatus"]).Replace("No idea", "NoIdea").Replace(" ", ""), out var ds))
                    {
                        me.DiscordStatus = (DiscordStatus)ds;

                        if(me.DiscordStatus == DiscordStatus.NoIdea
                            || me.DiscordStatus == DiscordStatus.Left
                            || me.DiscordStatus == DiscordStatus.CertifiedBadass)
                        {
                            me.IsActive = false;
                            me.ClanId = null;
                        }

                        if(me.DiscordStatus == 0
                            || me.DiscordStatus == DiscordStatus.Active)
                        {
                            me.IsActive = true;
                        }
                    }
                    else
                        me.IsActive = true;

                    if (row.Table.Columns.Contains("IgnoreOnMoveUp") && row["IgnoreOnMoveUp"] != DBNull.Value  && bool.TryParse((string)row["IgnoreOnMoveUp"], out var iomu))
                        me.IgnoreOnMoveUp = iomu;

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
            switch(tag)
            {
                case "TT": return "The Tavern";
                case "TT2": return "The Tavern 2";
                case "TT3": return "The Tavern 3";
            }
            return null;
        }

        public async Task<byte[]> ExportCsv(ulong guildID)
        {
            await using var mem = new MemoryStream();
            await using var writer = new StreamWriter(mem, Encoding.UTF8);
            await using var csvWriter = new CsvWriter(writer, CultureInfo.CurrentCulture);

            csvWriter.Configuration.Delimiter = ",";

            var m = _context.Member.AsEnumerable().Where(x => x.GuildId == guildID).ToList();
            await csvWriter.WriteRecordsAsync(m.ToArray().AsEnumerable());

            return mem.ToArray();
        }
    }
}
