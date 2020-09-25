using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTMMBot.Data.Entities;
using TTMMBot.Data.Enums;
using TTMMBot.Helpers;
using TTMMBot.Enums;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.InteropServices.ComTypes;

namespace TTMMBot.Services
{
    public class MemberSortService : IMemberSortService
    {
        private readonly IDatabaseService _databaseService;
        private readonly IGlobalSettingsService _globalSettings;

        public MemberSortService(IDatabaseService databaseService, IGlobalSettingsService globalSettings)
        {
            _databaseService = databaseService;
            _globalSettings = globalSettings;
        }

        public async Task<IList<MemberChanges>> GetChanges(ExchangeMode memberExchangeMode = ExchangeMode.SkipSteps)
        {
            var m = (await _databaseService.LoadMembersAsync()).Where(x => x.IsActive).ToList();
            var cqty = (await _databaseService.LoadClansAsync()).Count();

            var current = m.OrderBy(x => x.Clan?.Tag)
                .GroupBy(x => x.ClanId, (x, y) => new { Clan = x, Members = y })
                .Select(x => x.Members.ToList() as IList<Member>)
                .Select(x => x.OrderByDescending(y => y.SHigh).ToList())
                .ToList();

            var future = m.OrderByDescending(x => x.SHigh)
                .ToList()
                .ChunkBy(_globalSettings.ClanSize);

            if (current is null || future is null)
                return null;

            var moveQty = _globalSettings.MemberMovementQty;
            List<MemberChanges> result = null;

            switch(memberExchangeMode)
            {
                case ExchangeMode.OneStepUpAndDown:
                    result = GetOneStepMemberMovement(current, moveQty);
                    break;

                case ExchangeMode.SkipSteps:
                    result = await GetSkipStepsMemberMovement(m.OrderByDescending(y => y.SHigh).ToList(), moveQty, _globalSettings.ClanSize, cqty);
                    break;
            }

            return result;
        }

        private async Task<List<MemberChanges>> GetSkipStepsMemberMovement(List<Member> allMembersOrdered, int moveQty, int chunkSize, int clanQty)
            => await Task.Run(() =>
            {
                // get all members ordered
                var mpool = allMembersOrdered;

                // get all members ordered in clans
                var current = allMembersOrdered.OrderBy(x => x.Clan?.Tag)
                    .GroupBy(x => x.ClanId, (x, y) => new { Clan = x, Members = y })
                    .Select(x => x.Members.ToList() as IList<Member>)
                    .Select(x => x.OrderByDescending(y => y.SHigh).ToList())
                    .ToList();

                var ListOfLists = new List<List<Member>>();
                for(var i = 1; i <= clanQty; i++)
                {
                    var mL = new List<Member>();
                    foreach(var m in GetNextMemberForClan(mpool, i, moveQty, chunkSize))
                        mL.Add(m);

                    var removed = mpool.RemoveAll(x => mL.Contains(x));
                    if(mL.Count() != chunkSize)
                    {
                        var addLeader = current[i - 1].Where(x => x.Role >= Role.CoLeader && mpool.Contains(x));
                        mL.AddRange(addLeader);
                        removed += mpool.RemoveAll(x => mL.Contains(x));
                        
                        var addMissing = mpool.Where(x => x.Clan.SortOrder <= i).OrderByDescending(x => x.SHigh).Take(chunkSize - removed);
                        mL.AddRange(addMissing);
                        removed += mpool.RemoveAll(x => mL.Contains(x));

                        //var addMissing = current[i - 1].Where(x => mpool.Contains(x)).Take(chunkSize - removed);
                        //mL.AddRange(addMissing);
                        //removed += mpool.RemoveAll(x => mL.Contains(x));
                    }

                    //if(removed != chunkSize && mpool.Count > 0)
                        //throw new Exception();

                    ListOfLists.Add(mL);
                }

                return ListOfLists.Select((x, i) => new MemberChanges 
                { 
                    SortOrder = (i+1), // index starts at 0 so add 1
                    NewMemberList = x,
                    
                    Join = x.Where(y => !current[i].Contains(y)) // All new members which weren't in that clan before
                    .Select(y => new MemberMovement
                    {
                        Member = y,
                        IsUp = y.Clan.SortOrder > (i+1) // Is this join an up move?
                    }).ToList(),
                    Leave = current[i].Where(y => !x.Contains(y))
                    .Select(y => new MemberMovement{
                        Member = y,
                        IsUp = y.Clan.SortOrder > (i+1) // Is this leave an up move?
                    }).ToList()
                })
                .ToList();
            });

        private IEnumerable<Member> GetNextMemberForClan(List<Member> allMember, int clanSortNo, int moveQty, int chunkSize)
        {
            var currentSize = 0;
            var movedQty = 0;
            foreach(var currentMember in allMember)
            {
                if(movedQty >= moveQty || currentSize >= chunkSize)
                    break;

                if(clanSortNo < currentMember.Clan.SortOrder)
                {
                    if(currentMember.IgnoreOnMoveUp || currentMember.Role >= Role.CoLeader)
                        continue;
                    else
                        movedQty++;
                }

                currentSize++;
                yield return currentMember;
            } 
        }

        private List<MemberChanges> GetOneStepMemberMovement(List<List<Member>> current, int moveQty)
        {
            var highLowGroup = current
                            .Select((x, i) => new
                            {
                                ClanId = x.FirstOrDefault(x => x.ClanId.HasValue).ClanId.Value,
                                highest = x.OrderByDescending(x => x.SHigh).Where(x => !x.IgnoreOnMoveUp && x.Role < Role.CoLeader).Take(moveQty).ToList(),
                                lowest = x.OrderBy(x => x.SHigh).Where(x => x.Role < Role.CoLeader).Take(moveQty).ToList()
                            }).ToList();

            var result = highLowGroup
                .Select(x => new
                {
                    Curlow = x.lowest,
                    Nexthigh = highLowGroup.SkipWhile(y => y.ClanId != x.ClanId).Skip(1).FirstOrDefault()?.highest,
                    x.ClanId
                })
                .Select(x => new
                {
                    Curlow = x.Curlow ?? new List<Member>(),
                    Nexthigh = x.Nexthigh ?? new List<Member>(),
                    result = (x.Curlow ?? new List<Member>()).Concat(x.Nexthigh ?? new List<Member>()).OrderByDescending(y => y.SHigh).Take(moveQty),
                    x.ClanId
                })
                .Select((x, i) => new
                {
                    ClanId = x.ClanId,
                    Leave = x.Curlow.Where(y => x.result != null && !x.result.Contains(y)).ToList(),
                    Join = x.Nexthigh.Where(y => x.result.Contains(y)).ToList(),
                })
                .Select((x, i) => new MemberChanges
                {
                    SortOrder = x.ClanId,
                    Leave = x.Leave.Select(y => new MemberMovement { Member = y, IsUp = false }).ToList(),
                    Join = x.Join.Select(y => new MemberMovement { Member = y, IsUp = true }).ToList(),
                    NewMemberList = current[i].Concat(x.Join).Where(m => !x.Leave.Contains(m)).ToList()
                })
                .ToList();

            return result;
        }

        public async Task<IList<IList<Member>>> GetCurrentMemberList()
        {
            var m = (await _databaseService.LoadMembersAsync()).Where(x => x.IsActive).ToList();

            var sorted = m.GroupBy(x => x.ClanId, (x, y) => new { Clan = x, Members = y })
                    .OrderBy(x => x.Clan)
                    .Select(x => x.Members.OrderByDescending(x => x.SHigh).Select(v => v).ToList() as IList<Member>)
                    .ToList();

           return sorted;
        }

        public async Task<IList<IList<Member>>> GetSortedMemberList(SortMode sortedBy = SortMode.BySeasonHigh)
        {
            var m = (await _databaseService.LoadMembersAsync()).Where(x => x.IsActive).ToList();
            int? chunkSize = _globalSettings.ClanSize;

            switch(sortedBy)
            {
                case SortMode.BySeasonHigh:
                    m = m.OrderByDescending(x => x.SHigh).ToList();
                    break;

                case SortMode.ByAllTimeHigh:
                    m = m.OrderByDescending(x => x.AHigh).ToList();
                    break;
            }

            var sorted = m.ChunkBy(chunkSize.Value).ToList();
            return sorted;
        }

        public int GetSHighRank(IList<Member> allMembers, Member rankOf) => allMembers.OrderByDescending(x => x.SHigh).Select((m, i) => new { i, m }).FirstOrDefault(x => x.m == rankOf).i;
        public int GetFutureClan(IList<Member> allMembers, Member rankOf, int clanSize) => GetFutureClan(GetSHighRank(allMembers, rankOf), clanSize);
        public int GetFutureClan(int rank, int clanSize) => (int)Math.Ceiling(((double)rank/(double)clanSize));
    }
}
