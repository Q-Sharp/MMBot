﻿using System;
using TTMMBot.Data.Enums;

namespace TTMMBot.Data.Entities
{
    public class Member
    {
        public int MemberID { get; set; }

        public string Discord { get; set; }

        public string Name { get; set; }

        public int? AllTimeHigh { get; set; }

        public int? SeasonHighest { get; set; }

        public int? Donations { get; set; }

        public Role Role { get; set; }
        public bool IsActive { get; set; }

        public int? ClanID { get; set; }
        public Clan Clan { get; set; }

        public Vacation Vacation { get; set; }

        public DateTime? LastUpdated { get; set; }

        public override string ToString() => Clan?.Tag != null ? $"[{Clan?.Tag}] {Name}" : $"{Name}";
    }
}
