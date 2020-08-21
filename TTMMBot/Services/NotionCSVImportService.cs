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
        private Context _context { get; set; }
        private GlobalSettings _settings { get; set; }
        public NotionCSVImportService(Context context, GlobalSettings settings)
        {
            _context = context;
            _settings = settings;
        }

        public async Task<Exception> ImportCSV(byte[] csv)
        {                
            using var mem = new MemoryStream(csv);
            using var reader = new StreamReader(mem, Encoding.UTF8);
            using var csvReader = new CsvReader(reader, CultureInfo.CurrentCulture);

            csvReader.Configuration.HeaderValidated = null;
            csvReader.Configuration.MissingFieldFound = null;
            csvReader.Configuration.BadDataFound = null;
            csvReader.Configuration.Delimiter = ",";

            var c = _context.Clan;
            var m = _context.Member;

            try
            {
                if (c.Count() == 0)
                {
                    var nc = new Clan
                    {
                        Tag = "TT",
                        Name = "The Tavern"
                    };
                    await _context.AddAsync(nc);

                    for (var i = 2; i <= 4; i++)
                    {
                        var c2 = new Clan
                        {
                            Tag = $"TT{i}",
                            Name = $"The Tavern {i}"
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
                _settings.UseTriggers = false;

                using var dr = new CsvDataReader(csvReader);
                var dt = new DataTable();
                dt.Load(dr);

                foreach (DataRow row in dt.Rows)
                {
                    var me = m.FirstOrDefault(x => row["IGN"] != DBNull.Value && x.Name == (string)row["IGN"]) ?? new Member();

                    if (row["Clan"] != DBNull.Value && !string.IsNullOrEmpty((string)row["Clan"]))
                    { 
                        var clanid = c.FirstOrDefault(x => x.Tag == (string)row["Clan"])?.ClanID;
                        me.ClanID = clanid;
                    }

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
                        await _context.Member.AddAsync(me);
                }

                await _context.SaveChangesAsync();
                _settings.UseTriggers = true;
            }
            catch (Exception e)
            {
                return e;
            }

            return null;
        }
    }
}
