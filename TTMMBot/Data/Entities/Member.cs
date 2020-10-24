using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using TTMMBot.Data.Enums;

namespace TTMMBot.Data.Entities
{
    public class Member
    {
        [Key]
        public int MemberId { get; set; }

        [Required]
        [Display]
        public string Name { get; set; }

        [Display]
        [IgnoreDataMember]
        public string ClanTag => Clan?.Tag;

        [Display]
        public string Discord { get; set; }

        [Display]
        public int? AHigh { get; set; }

        [Display]
        public int? SHigh { get; set; }

        [Display]
        public int? Donations { get; set; }

        [Display]
        public Role Role { get; set; }

        public DiscordStatus DiscordStatus { get; set; } = DiscordStatus.Active;

        [Display]
        public bool IsActive { get; set; }

        public int? ClanId { get; set; }
        public virtual Clan Clan { get; set; }

        public virtual ICollection<Vacation> Vacation { get; set; }

        public DateTime? LastUpdated { get; set; }

        public int Join { get; set; }

        [Display]
        public bool IgnoreOnMoveUp { get; set; }

        [Display]
        public string PlayerTag { get; set; }

        //[Display]
        public bool AutoSignUpForFightNight { get; set; }

        public ulong GuildId { get; set; }

        public override string ToString() => Clan?.Tag != null ? $"[{Clan?.Tag}] {Name}" : $"{Name}";
    }
}
