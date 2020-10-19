using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.Extensions.Logging;
using TTMMBot.Data;
using TTMMBot.Data.Entities;
using TTMMBot.Data.Enums;
using TTMMBot.Services.Interfaces;

namespace TTMMBot.Services
{
    public class NotionCsvService : INotionCsvService
    {
        public Context Context { get; set; }
        public GlobalSettingsService Settings { get; set; }
        public ILogger<NotionCsvService> Logger { get; set; }

        public NotionCsvService(Context context, GlobalSettingsService settings, ILogger<NotionCsvService> logger)
        {
            Context = context;
            Settings = settings;
            Logger = logger;
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
                if (!Context.Clan.Any())
                {
                    var nc = new Clan
                    {
                        Tag = "TT",
                        Name = "The Tavern",
                        SortOrder = 1
                    };
                    await Context.AddAsync(nc);

                    for (var i = 2; i <= 4; i++)
                    {
                        var c2 = new Clan
                        {
                            Tag = $"TT{i}",
                            Name = $"The Tavern {i}",
                            SortOrder = i
                        };
                        await Context.AddAsync(c2);
                    }

                    await Context.SaveChangesAsync();
                }
            }
            catch
            {
                // ignore e
            }

            try
            {
                Settings.UseTriggers = Context.UseTriggers = false;

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
                    var me = Context.Member.FirstOrDefault(x => row["Name"] != DBNull.Value 
                        && x.Name.ToLower() == ((string)row["Name"]).ToLower());

                    if(me == null)
                    {
                        me = new Member();
                        await Context.Member.AddAsync(me);
                    }

                    if (row.Table.Columns.Contains("Name") && row["Name"] != DBNull.Value)
                        me.Name = (string)row["Name"];

                    if (row.Table.Columns.Contains("ClanTag") && row["ClanTag"] != DBNull.Value && !string.IsNullOrEmpty((string)row["ClanTag"]))
                    {
                        me.Clan = Context.Clan.FirstOrDefault(x => x.Tag == (string)row["ClanTag"]);
                        me.ClanId =  me.Clan?.ClanId;
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

                        if(me.DiscordStatus != DiscordStatus.Active)
                        {
                            me.IsActive = false;
                            me.ClanId = null;
                        }
                        else
                            me.IsActive = true;
                    }
                    else
                        me.IsActive = true;

                    if (row.Table.Columns.Contains("IgnoreOnMoveUp") && row["IgnoreOnMoveUp"] != DBNull.Value  && bool.TryParse((string)row["IgnoreOnMoveUp"], out var iomu))
                        me.IgnoreOnMoveUp = iomu;

                    me.LastUpdated = DateTime.Now;

                    await Context.SaveChangesAsync();
                }

                Settings.UseTriggers = null;
            }
            catch (Exception e)
            {
                return e;
            }

            return null;
        }

        public async Task<byte[]> ExportCsv()
        {
            await using var mem = new MemoryStream();
            await using var writer = new StreamWriter(mem, Encoding.UTF8);
            await using var csvWriter = new CsvWriter(writer, CultureInfo.CurrentCulture);

            csvWriter.Configuration.Delimiter = ",";

            var m = Context.Member;

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
    }
}
