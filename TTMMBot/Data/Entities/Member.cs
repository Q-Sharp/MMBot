using Discord;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TTMMBot.Data.Enums;

namespace TTMMBot.Data.Entities
{
    public class Member
    {
        public int MemberID { get; set; }

        public ulong DiscordID { get; set; }

        public string Discriminator { get; set; }

        public string Name { get; set; }

        public int AllTimeHigh { get; set; }

        public int SeasonHighest { get; set; }

        public int Donations { get; set; }

        public Role Role { get; set; }
        public bool IsActive { get; set; }

        public string ClanTag { get; set; }
        public Clan Clan { get; set; }

        public Vacation Vacation { get; set; }

        public DateTime? LastUpdated { get; set; }

        public override string ToString() => $"[{ClanTag}] {Name}";
    }
}
