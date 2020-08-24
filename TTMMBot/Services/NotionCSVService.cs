using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using TTMMBot.Data;
using TTMMBot.Data.Entities;
using TTMMBot.Data.Enums;

namespace TTMMBot.Services
{
    public class NotionCSVService
    {
        public Context Context { get; set; }
        public GlobalSettings Settings { get; set; }

        public NotionCSVService(Context context, GlobalSettings settings)
        {
            Context = context;
            Settings = settings;
        }

        public async Task<Exception> ImportCSV(byte[] csv)
        {
            await using var mem = new MemoryStream(csv);
            using var reader = new StreamReader(mem, Encoding.UTF8);
            using var csvReader = new CsvReader(reader, CultureInfo.CurrentCulture);

            csvReader.Configuration.HeaderValidated = null;
            csvReader.Configuration.MissingFieldFound = null;
            csvReader.Configuration.BadDataFound = null;
            csvReader.Configuration.Delimiter = ",";

            var c = Context.Clan.ToArray();
            var m = Context.Member.ToArray();

            try
            {
                if (!c.Any())
                {
                    var nc = new Clan
                    {
                        Tag = "TT",
                        Name = "The Tavern"
                    };
                    await Context.AddAsync(nc);

                    for (var i = 2; i <= 4; i++)
                    {
                        var c2 = new Clan
                        {
                            Tag = $"TT{i}",
                            Name = $"The Tavern {i}"
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

                foreach (DataRow row in dt.Rows)
                {
                    var me = m.FirstOrDefault(x => row["IGN"] != DBNull.Value && x.Name == (string)row["IGN"]) ?? new Member();

                    if (row["Clan"] != DBNull.Value && !string.IsNullOrEmpty((string)row["Clan"]))
                        me.ClanID = c.FirstOrDefault(x => x.Tag == (string)row["Clan"])?.ClanID;

                    if (row["Discord"] != DBNull.Value)
                        me.Discord = (string)row["Discord"];

                    if (row["IGN"] != DBNull.Value)
                        me.Name = (string)row["IGN"];

                    if (row["Role"] != DBNull.Value && Enum.TryParse(typeof(Role), ((string)row["Role"]).Replace("-", ""), out var er))
                        me.Role = (Role)er;

                    if (row["AT-highest"] != DBNull.Value && int.TryParse((string)row["AT-highest"], out var ath))
                        me.AHigh = ath;

                    if (row["S-highest"] != DBNull.Value && int.TryParse((string)row["S-highest"], out var sh))
                        me.SHigh = sh;

                    if (row["Donations"] != DBNull.Value && int.TryParse((string)row["Donations"], out var d))
                        me.Donations = d;

                    if (row["Join Date"] != DBNull.Value && int.TryParse((string)row["Join Date"], out var jd))
                        me.Join = jd;

                    me.IsActive = me.ClanID.HasValue && me.SHigh.HasValue;
                    me.LastUpdated = DateTime.Now;

                    if(me.MemberID == 0)
                        await Context.Member.AddAsync(me);
                }

                await Context.SaveChangesAsync();
                Settings.UseTriggers = Context.UseTriggers = true;
            }
            catch (Exception e)
            {
                return e;
            }

            return null;
        }

        public async Task<byte[]> ExportCSV()
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
