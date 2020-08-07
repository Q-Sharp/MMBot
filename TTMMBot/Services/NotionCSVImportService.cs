using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using TTMMBot.Data.Enums;

namespace TTMMBot.Services
{
    public class NotionCSVImportService
    {
        public IDatabaseService DatabaseService { get; set; }
        public NotionCSVImportService(IDatabaseService databaseService) => DatabaseService = databaseService;

        public async Task<bool> ImportCSV(byte[] csv)
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
                var c = await DatabaseService.CreateClanAsync();
                c.Tag = "TT";
                c.Name = "The Tavern";
                await DatabaseService.SaveDataAsync();

                for(var i = 2; i <= 4; i++)
                {
                    var c2 = await DatabaseService.CreateClanAsync();
                    c2.Tag = $"TT{i}";
                    c2.Name = $"The Tavern {i}";
                    await DatabaseService.SaveDataAsync();
                }
            }
            catch
            {
                // ignore e
            }

            var clans = await DatabaseService.LoadClansAsync();
            var members = await DatabaseService.LoadMembersAsync();

            try
            {
                using var dr = new CsvDataReader(csvReader);
                var dt = new DataTable();
                dt.Columns.Add("ClanID", typeof(string));
                dt.Load(dr);

                foreach (DataRow row in dt.Rows)
                {
                    if (row["Clan"] == DBNull.Value)
                        continue;

                    
                    var clanid = clans.FirstOrDefault(x => x.Tag == (string)row["Clan"])?.ClanID;
                    if (clanid != null)
                        row["ClanID"] = clanid.ToString();
                }

                foreach (DataRow row in dt.Rows)
                {
                    var m = members.FirstOrDefault(x => row["IGN"] != DBNull.Value && x.Name == (string)row["IGN"]) ?? await DatabaseService.CreateMemberAsync();

                    if (row["Discord"] != DBNull.Value)
                        m.Discord = (string)row["Discord"];

                    if (row["IGN"] != DBNull.Value)
                        m.Name = (string)row["IGN"];

                    if (row["Role"] != DBNull.Value && Enum.TryParse(typeof(Role), ((string)row["Role"]).Replace("-", ""), out var er))
                        m.Role = (Role)er;

                    if (row["ClanID"] != DBNull.Value && int.TryParse((string)row["ClanID"], out var cid))
                        m.ClanID = cid;

                    if (row["AT-highest"] != DBNull.Value && int.TryParse((string)row["AT-highest"], out var ath))
                        m.AllTimeHigh = ath;

                    if (row["S-highest"] != DBNull.Value && int.TryParse((string)row["S-highest"], out var sh))
                        m.SeasonHighest = sh;

                    if (row["Donations"] != DBNull.Value && int.TryParse((string)row["Donations"], out var d))
                        m.Donations = d;

                    m.IsActive = m.ClanID.HasValue;
                    m.LastUpdated = DateTime.Now;
                }

                //m.LastUpdated = DateTime.Parse((string)row["LastUpdate"]);
                //m.ToList().ForEach(m => m.Clan = c.Clan.FirstOrDefault(d => d.Tag == m.Clan.Tag));
                await DatabaseService.SaveDataAsync();
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
