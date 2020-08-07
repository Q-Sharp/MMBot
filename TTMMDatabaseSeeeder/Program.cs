﻿using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using TTMMBot.Data;
using TTMMBot.Data.Entities;
using TTMMBot.Data.Enums;

namespace TTMMDatabaseSeeeder
{
    class Program
    {
        static void Main(string[] args)
        {
            using var c = new Context();
            c.Database.Migrate();

            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream(assembly.GetManifestResourceNames()[0]);
            using var reader = new StreamReader(stream, Encoding.UTF8);

            var csvReader = new CsvReader(reader, CultureInfo.CurrentCulture);

            csvReader.Configuration.HeaderValidated = null;
            csvReader.Configuration.MissingFieldFound = null;
            csvReader.Configuration.BadDataFound = null;
            csvReader.Configuration.Delimiter = ",";

            try
            {
                c.Clan.Add(new Clan { Tag = "TT", Name = "The Tavern" });
                c.Clan.Add(new Clan { Tag = "TT2", Name = "The Tavern 2" });
                c.Clan.Add(new Clan { Tag = "TT3", Name = "The Tavern 3" });
                c.Clan.Add(new Clan { Tag = "TT4", Name = "The Tavern 4" });
                c.SaveChanges();
            }
            catch
            {
            }

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

                    var clanid = c.Clan.FirstOrDefault(x => x.Tag == (string)row["Clan"])?.ClanID;
                    if (clanid != null)
                        row["ClanID"] = clanid.ToString();
                }

                foreach (DataRow row in dt.Rows)
                {
                    Member m = c.Member.FirstOrDefault(x => row["IGN"] != DBNull.Value && x.Name == (string)row["IGN"]) ?? new Member();

                    if (row["Discord"] != DBNull.Value)
                        m.Discord = (string)row["Discord"];

                    if (row["IGN"] != DBNull.Value)
                        m.Name = (string)row["IGN"];

                    if (row["Role"] != DBNull.Value && Enum.TryParse(typeof(Role), ((string)row["Role"]).Replace("-", ""), out var er))
                        m.Role = (Role)er;

                    if (row["ClanID"] != DBNull.Value && int.TryParse((string)row["ClanID"], out var cid))
                        m.AllTimeHigh = cid;

                    if (row["AT-highest"] != DBNull.Value && int.TryParse((string)row["AT-highest"], out var ath))
                        m.AllTimeHigh = ath;

                    if (row["S-highest"] != DBNull.Value && int.TryParse((string)row["S-highest"], out var sh))
                        m.SeasonHighest = sh;

                    if (row["Donations"] != DBNull.Value && int.TryParse((string)row["Donations"], out var d))
                        m.Donations = d;

                    m.IsActive = m.ClanID.HasValue ? true : false;
                    m.LastUpdated = DateTime.Now;

                    c.Member.Update(m);
                    Console.WriteLine($"{m} updated!");
                }

                //m.LastUpdated = DateTime.Parse((string)row["LastUpdate"]);
                //m.ToList().ForEach(m => m.Clan = c.Clan.FirstOrDefault(d => d.Tag == m.Clan.Tag));
                c.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
