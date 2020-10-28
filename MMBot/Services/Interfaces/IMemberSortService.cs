using System.Collections.Generic;
using System.Threading.Tasks;
using MMBot.Data.Entities;
using MMBot.Enums;
using MMBot.Services.MemberSort;

namespace MMBot.Services.Interfaces
{
    public interface IMemberSortService : IMMBotInterface
    {
        public Task<IList<IList<Member>>> GetCurrentMemberList();

        public Task<IList<IList<Member>>> GetSortedMemberList(SortMode sortedBy = SortMode.BySeasonHigh);

        public Task<IList<MemberChanges>> GetChanges(ExchangeMode memberExchangeMode = ExchangeMode.SkipSteps);
    }
}
