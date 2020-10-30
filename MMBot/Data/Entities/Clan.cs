using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MMBot.Helpers;

namespace MMBot.Data.Entities
{
    public class Clan : IHaveId
    {
        [Key]
        public int Id { get; set; }

        public int SortOrder { get; set; }

        [Required]
        [Display]
        [ConcurrencyCheck]
        public string Tag { get; set; }

        [Display]
        public string Name { get; set; }
        public string DiscordRole { get; set; }

        public ulong GuildId { get; set; }

        public virtual ICollection<Member> Member { get; set; }
        public override string ToString() => $"[{Tag}]";

        public void Update(object clan)
        {
            if(clan is Clan c && (Id == c.Id || Name == c.Name))
                this.ChangeProperties(c);
        }
    }
}
