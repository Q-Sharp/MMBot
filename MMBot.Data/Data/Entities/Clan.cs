using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MMBot.Data.Interfaces;

namespace MMBot.Data.Entities
{
    public class Clan : IHaveId, IHaveIdentifier, IHaveGuildId
    {
        public virtual int Id { get; set; }

        public virtual int SortOrder { get; set; }

        [Required]
        [Display]
        public virtual string Tag { get; set; }

        [JsonIgnore]
        public virtual string Identitfier => Tag;

        [Display]
        public virtual string Name { get; set; }
        public virtual string DiscordRole { get; set; }

        public virtual ulong GuildId { get; set; }

        [JsonIgnore]
        public virtual ICollection<Member> Member { get; set; } = new Collection<Member>();
        public override string ToString() => $"[{Tag}]";

        public void Update(object clan)
        {
            if(clan is Clan c && (Id == c.Id || Name == c.Name))
            {
                SortOrder = c.SortOrder;
                Tag = c.Tag;
                Name = c.Name;
                DiscordRole = c.DiscordRole;
                GuildId = c.GuildId;
            }
        }
    }
}
