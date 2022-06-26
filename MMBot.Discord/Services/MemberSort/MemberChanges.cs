using MMBot.Data.Contracts.Entities;

namespace MMBot.Discord.Services.MemberSort;

public class MemberChanges
{
    public int SortOrder { get; set; }
    public IList<MemberMovement> Leave { get; set; }
    public IList<MemberMovement> Join { get; set; }
    public IList<Member> NewMemberList { get; set; }
}
