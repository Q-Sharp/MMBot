namespace MMBot.DSharp.Contracts.Services;

public interface IMemberSortService
{
    public Task<IList<IList<Member>>> GetCurrentMemberList(ulong guildId);

    public Task<IList<IList<Member>>> GetSortedMemberList(ulong guildId, SortMode sortedBy = SortMode.BySeasonHigh);

    //public Task<IList<MemberChanges>> GetChanges(ulong guildId, bool useCurrent = false, ExchangeMode memberExchangeMode = ExchangeMode.SkipSteps);
}
