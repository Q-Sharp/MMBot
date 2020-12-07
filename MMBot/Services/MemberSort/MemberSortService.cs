using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MMBot.Data.Entities;
using MMBot.Data.Enums;
using MMBot.Helpers;
using MMBot.Enums;
using MMBot.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace MMBot.Services.MemberSort
{
    public class MemberSortService : MMBotService<MemberSortService>, IMemberSortService
    {
        private readonly IDatabaseService _databaseService;
        private readonly IGuildSettingsService _guildSettings;

        public MemberSortService(IDatabaseService databaseService, IGuildSettingsService guildSettings, ILogger<MemberSortService> logger) : base(logger)
        {
            _databaseService = databaseService;
            _guildSettings = guildSettings;
        }

        public async Task<IList<MemberChanges>> GetChanges(ulong guildId, bool useCurrent = false, ExchangeMode memberExchangeMode = ExchangeMode.SkipSteps)
        {
            var settings = await _guildSettings.GetGuildSettingsAsync(guildId);
            var m = (await _databaseService.LoadMembersAsync(guildId)).Where(x => x.IsActive).ToList();
            var cqty = (await _databaseService.LoadClansAsync(guildId)).Count;

            var current = m.OrderBy(x => x.Clan?.Tag)
                .GroupBy(x => x.ClanId, (x, y) => new { Clan = x, Members = y })
                .Select(x => x.Members.ToList() as IList<Member>)
                .Select(x => x.OrderByDescending(y => y.SHigh).ToList())
                .ToList();

            var future = m.OrderByDescending(x => (useCurrent ? x.Current : x?.SHighLowest ?? x.SHigh))
                .ToList()
                .ChunkBy(settings.ClanSize);

            if (current is null || future is null)
                return default;

            var moveQty = settings.MemberMovementQty;
            List<MemberChanges> result = null;

            switch(memberExchangeMode)
            {
                case ExchangeMode.OneStepUpAndDown:
                    result = GetOneStepMemberMovement(current, moveQty);
                    break;

                case ExchangeMode.SkipSteps:
                    result = await GetSkipStepsMemberMovement(m.OrderByDescending(y => (useCurrent ? y.Current : y?.SHighLowest ?? y.SHigh))
                        .Where(x => x.IsActive && x.Role != Role.ExMember).ToList(), moveQty, settings.ClanSize, cqty, useCurrent);
                    break;
            }

            return result;
        }

        private static async Task<List<MemberChanges>> GetSkipStepsMemberMovement(List<Member> allMembersOrdered, int moveQty, int chunkSize, int clanQty, bool useCurrent = false)
            => await Task.Run(() =>
            {
                // get all members ordered
                var mpool = allMembersOrdered;

                // get all members ordered in clans
                var current = allMembersOrdered.OrderBy(x => x.Clan?.Tag)
                    .GroupBy(x => x.ClanId, (x, y) => new { Clan = x, Members = y })
                    .Select(x => x.Members.ToList() as IList<Member>)
                    .Select(x => x.OrderByDescending(y => (useCurrent ? y.Current : y?.SHighLowest ?? y.SHigh)).ToList())
                    .ToList();

                var ListOfLists = new List<List<Member>>();
                for(var i = 1; i <= clanQty; i++)
                {
                    var mL = new List<Member>();

                    var addLeader = current[i - 1].Where(x => x.Role >= Role.CoLeader && mpool.Contains(x));
                    mL.AddRange(addLeader);
                    var removed = mpool.RemoveAll(x => mL.Contains(x));

                    foreach(var m in GetNextMemberForClan(mpool, i, removed, moveQty, chunkSize, useCurrent))
                        mL.Add(m);

                    removed += mpool.RemoveAll(x => mL.Contains(x));
                    if(mL.Count != chunkSize)
                    {
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

        private static IEnumerable<Member> GetNextMemberForClan(List<Member> allMember, int clanSortNo, int currentSize, int moveQty, int chunkSize, bool useCurrent = false)
        {
            var movedQty = 0;
            foreach(var currentMember in allMember)
            {
                if(movedQty >= moveQty || currentSize >= chunkSize)
                    break;

                if(clanSortNo < currentMember.Clan.SortOrder)
                {
                    if(!useCurrent && currentMember.IgnoreOnMoveUp || currentMember.Role >= Role.CoLeader || CheckMemberGroup(currentMember, allMember, clanSortNo))
                        continue;
                    else
                        movedQty++;
                }

                currentSize++;
                yield return currentMember;
            } 
        }

        private static bool CheckMemberGroup(Member currentMember, List<Member> allMember, int clanSortNo)
        {
            if(!currentMember.MemberGroupId.HasValue)
                return false;

            try
            {
                var member = currentMember.MemberGroup.Members;
                var newM = allMember.Where(x => member.Select(y => y.Id).Contains(x.Id));
                return newM.Any(x => x.Clan.SortOrder > clanSortNo);
            }
            catch
            {
                return false;
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
                    Leave = x.Curlow.Where(y => x.result is not null && !x.result.Contains(y)).ToList(),
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

        public async Task<IList<IList<Member>>> GetCurrentMemberList(ulong guildId)
        {
            var m = (await _databaseService.LoadMembersAsync(guildId)).Where(x => x.IsActive).ToList();

            var sorted = m.GroupBy(x => x.ClanId, (x, y) => new { Clan = x, Members = y })
                    .OrderBy(x => x.Clan)
                    .Select(x => x.Members.OrderByDescending(x => x?.SHigh ?? x.Id).ToList() as IList<Member>)
                    .ToList();

           return sorted;
        }

        public async Task<IList<IList<Member>>> GetSortedMemberList(ulong guildId, SortMode sortedBy = SortMode.BySeasonHigh)
        {
            var settings = await _guildSettings.GetGuildSettingsAsync(guildId);
            var m = (await _databaseService.LoadMembersAsync(guildId)).Where(x => x.IsActive).ToList();
            int? chunkSize = settings.ClanSize;

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
