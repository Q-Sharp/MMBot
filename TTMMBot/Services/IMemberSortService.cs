using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TTMMBot.Data.Entities;
using TTMMBot.Enums;

namespace TTMMBot.Services
{
    public class MemberChanges
    {
        public int SortOrder { get; set; }
        public IList<MemberMovement> Leave { get; set; }
        public IList<MemberMovement> Join { get; set; }
        public IList<Member> NewMemberList { get; set; }
    }

    public class MemberMovement
    {
        public Member Member { get; set; }
        public bool IsUp { get; set; }
    }

    public interface IMemberSortService
    {
        public Task<IList<IList<Member>>> GetCurrentMemberList();

        public Task<IList<IList<Member>>> GetSortedMemberList(SortMode sortedBy = SortMode.BySeasonHigh);

        public Task<IList<MemberChanges>> GetChanges(ExchangeMode memberExchangeMode = ExchangeMode.SkipSteps);
    }
}
