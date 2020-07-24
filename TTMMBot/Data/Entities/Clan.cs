using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TTMMBot.Data.Entities
{
    public class Clan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ClanID { get; set; }

        public string Name { get; set; }

        public ICollection<Member> Members { get; set; }
    }
}
