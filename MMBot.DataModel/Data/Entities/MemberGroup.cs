using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using MMBot.Data.Interfaces;

namespace MMBot.Data.Entities
{
    public class MemberGroup : IHaveId
    {
        [Key]
        public int Id { get; set; }

        [JsonIgnore]
        public virtual ICollection<Member> Members { get; set; } = new Collection<Member>();

        [JsonIgnore]
        [Display]
        public int? SHighLowest => Members?.Min(x => x.SHigh);

        public void Update(object memberGroup)
        {
        }
    }
}
