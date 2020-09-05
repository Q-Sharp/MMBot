using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TTMMBot.Data.Entities
{
    public class Clan
    {
        [Key]
        public int ClanId { get; set; }

        public int SortOrder { get; set; }

        [Required]
        [Display]
        public string Tag { get; set; }

        [Display]
        public string Name { get; set; }
        public string DiscordRole { get; set; }

        public virtual ICollection<Member> Member { get; set; }
        public override string ToString() => $"[{Tag}]";
    }
}
