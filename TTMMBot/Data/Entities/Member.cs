using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TTMMBot.Data.Enums;

namespace TTMMBot.Data.Entities
{
    public class Member
    {
        [Key]
        public int MemberID { get; set; }

        public string Discord { get; set; }

        [Required]
        public string Name { get; set; }

        public int? AllTimeHigh { get; set; }

        public int? SeasonHighest { get; set; }

        public int? Donations { get; set; }

        [NotMapped]
        public int? TotalScore => (5 * SeasonHighest + 2 * AllTimeHigh + 1 * Donations) / 3;

        public Role Role { get; set; }
        public bool IsActive { get; set; }

        public int? ClanID { get; set; }
        public virtual Clan Clan { get; set; }

        public virtual ICollection<Vacation> Vacation { get; set; }

        public DateTime? LastUpdated { get; set; }

        public override string ToString() => Clan?.Tag != null ? $"[{Clan?.Tag}] {Name}" : $"{Name}";
    }
}
