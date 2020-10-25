using System.Collections.Generic;
using System.Threading.Tasks;
using TTMMBot.Data.Entities;
using TTMMBot.Enums;
using TTMMBot.Services.MemberSort;

namespace TTMMBot.Services.Interfaces
{
    public interface IMemberSortService : IMMBotInterface
    {
        public Task<IList<IList<Member>>> GetCurrentMemberList();

        public Task<IList<IList<Member>>> GetSortedMemberList(SortMode sortedBy = SortMode.BySeasonHigh);

        public Task<IList<MemberChanges>> GetChanges(ExchangeMode memberExchangeMode = ExchangeMode.SkipSteps);
    }
}
