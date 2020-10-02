using System;
using System.Linq;
using TTMMBot.Data;
using TTMMBot.Data.Entities;
using TTMMBot.Services.Interfaces;

namespace TTMMBot.Services
{
    public class GlobalSettingsService : IGlobalSettingsService
    {
        public Context Dbcontext { get; set; }
        private GlobalSettings Gs => Dbcontext?.GlobalSettings.FirstOrDefault();

        public GlobalSettingsService(Context dbcontext)
        {
            Dbcontext = dbcontext;

            if (!Dbcontext?.GlobalSettings?.AsQueryable()?.Any() ?? false)
            {
                var gs = new GlobalSettings
                {
                    Prefix = "m.",
                    WaitForReaction = TimeSpan.FromMinutes(5),
                    UseTriggers = true,
                    FileName = "export.csv",
                    ClanSize = 20
                };

                Dbcontext.GlobalSettings.Add(gs);
                Dbcontext?.SaveChanges();
            }
        }

        // Discord
        public string Prefix => Gs.Prefix;
        public TimeSpan WaitForReaction => Gs.WaitForReaction;

        // Database
        public bool? UseTriggers 
        {
            get => _tempUseTriggers ?? Gs.UseTriggers;
            set => _tempUseTriggers = value;
        }
        private bool? _tempUseTriggers;

        // Filesystem
        public string FileName => Gs.FileName;

        // InGame
        public int ClanSize => Gs.ClanSize;
        public int MemberMovementQty => Gs.MemberMovementQty;
    }
}
