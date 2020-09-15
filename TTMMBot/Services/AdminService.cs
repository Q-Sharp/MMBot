using System;
using System.Collections.Generic;
using System.Linq;
using TTMMBot.Data;
using TTMMBot.Data.Entities;

namespace TTMMBot.Services
{
    public class AdminService : IAdminService
    {
        public Context Context { get; set; }
        public IGlobalSettingsService Settings { get; set; }
        public ICommandHandler CommandHandler { get; set; }

        public AdminService(Context context, IGlobalSettingsService settings, ICommandHandler commandHandler)
        {
            Context = context;
            Settings = settings;
            CommandHandler = commandHandler;
        }

        public class JoinComparer : IComparer<int>
        {
            public static JoinComparer Create() => new JoinComparer();

            public int Compare(int x, int y)
            {
                var nx = x;
                var ny = y;

                if (x == 0)
                    nx = 21;

                if (y == 0)
                    ny = 21;

                return nx - ny;
            }
        }

        public void Reorder()
        {
            Context.Member.AsEnumerable()
                .OrderBy(x => x.Join, JoinComparer.Create())
                .ThenBy(x => x.SHigh)
                .GroupBy(x => x.ClanId, (x, y) => new { Clan = x, Members = y })
                .Select(x => x.Members.ToList() as IList<Member>)
                .ToList()
                .ForEach(x =>
                {
                    var i = 1;
                    x.ToList().ForEach(m => m.Join = i++);
                });

            Context?.SaveChanges();
        }

        //public string GetDeletedMessages() 
        //{
        //    var dm = CommandHandler?.DeletedMessages;
        //    var dms = dm.Select(x => $"{x.Author} wrote {x.Content} in {x.Channel} on {x.Timestamp}");

        //    return string.Join(Environment.NewLine, dms);
        //}

        //public IEnumerable<string> GetDeletedMessages()
        //{
        //   var dm = CommandHandler?.DeletedMessages;
        //    foreach(var sdm in dm)
        //        yield return $"{sdm.Author} wrote {sdm.Content} in {sdm.Channel} on {sdm.Timestamp}";
        //}
    }
}