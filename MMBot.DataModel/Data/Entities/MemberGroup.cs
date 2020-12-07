using System.Text.Json.Serialization;
using MMBot.Data.Interfaces;
using System.Collections.Generic;

namespace MMBot.Data.Entities
{
    public class MemberGroup : IHaveId
    {
        public int Id { get; set; }

        [JsonIgnore]
        public virtual IList<Member> Members { get; set; } = new List<Member>();

        public void Update(object memberGroup)
        {
        }
    }
}
