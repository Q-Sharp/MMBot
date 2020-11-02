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
        private ulong _guildId;

        public CsvService(Context context, GuildSettingsService settings, ILogger<CsvService> logger)
        {
            _context = context;
            _settings = settings;
            _logger = logger;
        }

        public async Task<Exception> ImportCsv(byte[] csv)
        {
            using var mem = new MemoryStream(csv);
            using var reader = new StreamReader(mem, Encoding.UTF8);
            using var csvReader = new CsvReader(reader, CultureInfo.CurrentCulture);

            csvReader.Configuration.HeaderValidated = null;
            csvReader.Configuration.MissingFieldFound = null;
            csvReader.Configuration.BadDataFound = null;
            csvReader.Configuration.Delimiter = ",";

            try
            {
                if (!_context.Clan.Any())
                {
                    var nc = new Clan
                    {
                        Tag = "TT",
                        Name = "The Tavern",
                        SortOrder = 1,
                        GuildId = _settings.GuildId
                    };
                    await _context.AddAsync(nc);

                    for (var i = 2; i <= 4; i++)
                    {
                        var c2 = new Clan
                        {
                            Tag = $"TT{i}",
                            Name = $"The Tavern {i}",
                            SortOrder = i,
                            GuildId = _settings.GuildId
                        };
                        await _context.AddAsync(c2);
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
                
                foreach (DataRow row in dt.Rows)
                {
                    var me = row["Name"] != DBNull.Value && _context.Member.Any() ? _context.Member.FirstOrDefault(
                        x => x.Name.ToLower() == ((string)row["Name"]).ToLower()) : null;

                    if(me == null)
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

                    if (row.Table.Columns.Contains("IgnoreOnMoveUp") && row["IgnoreOnMoveUp"] != DBNull.Value  && bool.TryParse((string)row["IgnoreOnMoveUp"], out var iomu))
                        me.IgnoreOnMoveUp = iomu;

                    me.LastUpdated = DateTime.UtcNow;
                    me.GuildId = _settings.GuildId;

                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                return e;
            }

            return default;
        }

        public async Task<byte[]> ExportCsv()
        {
            await using var mem = new MemoryStream();
            await using var writer = new StreamWriter(mem, Encoding.UTF8);
            await using var csvWriter = new CsvWriter(writer, CultureInfo.CurrentCulture);

            csvWriter.Configuration.Delimiter = ",";

            var m = _context.Member;

            //var dt = new DataTable();
            //var dc = new DataColumn[]
            //{
            //    new DataColumn("Clan", typeof(string)),
            //    new DataColumn("Discord", typeof(string)),
            //    new DataColumn("IGN", typeof(string)),
            //    new DataColumn("Role", typeof(string)),
            //    new DataColumn("AT-highest", typeof(string)),
            //    new DataColumn("S-highest", typeof(string)),
            //    new DataColumn("Donations", typeof(string)),
            //    new DataColumn("Join Date", typeof(string)),
            //};
            //dt.Columns.AddRange(dc);

            //foreach (var member in m.ToArray())
            //    dt.Rows.Add(member.ClanTag, member.Discord, member.Name, member.Role.ToString(),
            //        member.AHigh.ToString(), member.SHigh.ToString(), member.Donations.ToString(),
            //        member.Join.ToString());

            await csvWriter.WriteRecordsAsync(m.ToArray().AsEnumerable());

            return mem.ToArray();
        }

        public void SetGuild(ulong id) => _guildId = id;
    }
}
