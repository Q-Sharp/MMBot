using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TTMMBot.Data.Entities
{
    public class Member
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MemberID { get; set; }

        public int DiscordID { get; set; }

        public string Name { get; set; }
        
        public int AllTimeHigh { get; set; }

        public int SeasonHighest { get; set; }

        public int ClanID { get; set; }
        public Clan Clan { get; set; }
    }
}
