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
    public class NotionCSVImportService
    {
        public Context Context { get; set; }
        public GlobalSettings Settings { get; set; }

        public NotionCSVImportService(Context context, GlobalSettings settings)
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

            var c = Context.Clan;
            var m = Context.Member;

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
                Settings.UseTriggers = false;

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
                Settings.UseTriggers = true;
            }
            catch (Exception e)
            {
                return e;
            }

            return null;
        }
    }
}
