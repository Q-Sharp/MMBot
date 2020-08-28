using System;

namespace TTMMBot
{
    public class GlobalSettings : IGlobalSettings
    {
        // Discord
        public string Prefix { get; set; } = "m.";
        public TimeSpan WaitForReaction { get; set; } = TimeSpan.FromMinutes(5);

        // Database
        public bool UseTriggers { get; set; } = true;

        // Filesystem
        public string FileName { get; set; } = "export.csv";

        // InGame
        public int ClanSize { get; set; } = 20;
    }
}
