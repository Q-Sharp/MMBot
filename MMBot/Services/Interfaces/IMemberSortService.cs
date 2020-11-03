using System.Collections.Generic;
using System.Threading.Tasks;
using MMBot.Data.Entities;
using MMBot.Enums;
using MMBot.Services.MemberSort;

namespace MMBot.Services.Interfaces
{
    public interface IMemberSortService
    {
        public Task<IList<IList<Member>>> GetCurrentMemberList(ulong guildId);

        public Task<IList<IList<Member>>> GetSortedMemberList(ulong guildId, SortMode sortedBy = SortMode.BySeasonHigh);

        public Task<IList<MemberChanges>> GetChanges(ulong guildId, ExchangeMode memberExchangeMode = ExchangeMode.SkipSteps);
    }
}
