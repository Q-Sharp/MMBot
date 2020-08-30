using System;
using System.ComponentModel.DataAnnotations;

namespace TTMMBot.Data.Entities
{
    public class GlobalSettings
    {
        [Key]
        public int GlobalSettingsId { get; set; }

        // Discord
        [Display]
        public string Prefix { get; set; } = "m.";

        [Display]
        public TimeSpan WaitForReaction { get; set; } = TimeSpan.FromMinutes(5);

        // Database
        public bool UseTriggers { get; set; } = true;

        // Filesystem
        [Display]
        public string FileName { get; set; } = "export.csv";

        // InGame
        [Display]
        public int ClanSize { get; set; } = 20;
    }
}
