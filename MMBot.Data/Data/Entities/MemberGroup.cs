using System.Text.Json.Serialization;
using MMBot.Data.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MMBot.Data.Entities
{
    public class MemberGroup : IHaveId
    {
        public int Id { get; set; }

        [JsonIgnore]
        public virtual ICollection<Member> Members { get; set; } = new Collection<Member>();

        public void Update(object memberGroup)
        {
        }
    }
}
