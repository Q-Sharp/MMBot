using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TTMMBot.Data.Entities
{
    public class Clan
    {
        [Key]
        public int ClanID { get; set; }

        [Required]
        public string Tag { get; set; }

        public string Name { get; set; }
        public string DiscordRole { get; set; }

        public virtual ICollection<Member> Member { get; set; }
        public override string ToString() => $"[{Tag}]";
    }
}
