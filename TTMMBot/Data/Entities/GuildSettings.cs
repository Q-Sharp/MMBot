using System;
using System.ComponentModel.DataAnnotations;

namespace TTMMBot.Data.Entities
{
    public class GuildSettings
    {
        [Key]
        public int GuildSettingsId { get; set; }

        public ulong GuildId { get; set; }

        // Discord
        [Display]
        public string Prefix { get; set; }

        [Display]
        public TimeSpan WaitForReaction { get; set; }

        // Filesystem
        [Display]
        public string FileName { get; set; }

        // InGame
        [Display]
        public int ClanSize { get; set; }

        [Display]
        public int MemberMovementQty { get; set; }

        public bool? UseTriggers { get; set; }
    }
}
