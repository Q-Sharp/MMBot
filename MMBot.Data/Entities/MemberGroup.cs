using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using MMBot.Data.Interfaces;

namespace MMBot.Data.Entities;

public class MemberGroup : IHaveId
{
    public int Id { get; set; }

    [JsonIgnore]
    public virtual ICollection<Member> Members { get; set; } = new Collection<Member>();

    public void Update(object memberGroup)
    {
    }
}
