namespace MMBot.Data.Contracts.Entities;

public class MemberGroup : IHaveId
{
    public int Id { get; set; }

    [JsonIgnore]
    public virtual ICollection<Member> Members { get; set; } = new Collection<Member>();

    public byte[] Version { get; set; }
    public object Update(object memberGroup)
    {
        return this;
    }
}
