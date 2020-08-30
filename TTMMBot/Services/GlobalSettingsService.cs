﻿using System;
using System.Linq;
using TTMMBot.Data;
using TTMMBot.Data.Entities;

namespace TTMMBot.Services
{
    public class GlobalSettingsService
    {
        private readonly Context _dbcontext;
        private GlobalSettings Gs => _dbcontext?.GlobalSettings.FirstOrDefault();

        public GlobalSettingsService(Context dbcontext)
        {
            _dbcontext = dbcontext;

            if (!dbcontext.GlobalSettings.AsQueryable().Any())
            {
                var gs = new GlobalSettings
                {
                    Prefix = "m.",
                    WaitForReaction = TimeSpan.FromMinutes(5),
                    UseTriggers = true,
                    FileName = "export.csv",
                    ClanSize = 20
                };

                dbcontext.GlobalSettings.Add(gs);
                dbcontext?.SaveChanges();
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
    }
}
