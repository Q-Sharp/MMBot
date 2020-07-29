using CsvHelper;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using TTMMBot.Data;
using TTMMBot.Data.Entities;

namespace TTMMDatabaseSeeeder
{
    class Program
    {
        static void Main(string[] args)
        {
            using var c = new Context();

            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream(assembly.GetManifestResourceNames()[0]);
            using var reader = new StreamReader(stream, Encoding.UTF8);

            var csvReader = new CsvReader(reader, CultureInfo.CurrentCulture);

            csvReader.Configuration.HeaderValidated = null;
            csvReader.Configuration.MissingFieldFound = null;
            var m = csvReader.GetRecords<Member>().ToArray();

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

            m.ToList().ForEach(m => m.Clan = c.Clan.FirstOrDefault(d => d.Tag == m.ClanTag));

            c.Member.AddRange(m);

            c.SaveChanges();
        }
    }
}
