using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using MMBot.Helpers;
using MMBot.Modules.Énum;
using MMBot.Modules.Interfaces;
using MMBot.Services.MemberSort;

namespace MMBot.Modules.Member
{
    public partial class MemberModule : MMBotModule, IMemberModule
    {
        private readonly string[] _header = { "Name", "Clan", "SHigh", "Role" };
        private readonly int[] _pad = { 16, 4, 5, 7 };
        private readonly string[] _fields = { "Name", "Clan.Tag", "SHigh", "Role" };

        [Command("List", RunMode = RunMode.Async)]
        [Summary("Lists all members by current clan membership.")]
        public async Task<RuntimeResult> List(SortBy sortBy = SortBy.SHigh)
        {
            var m = await _memberSortService.GetCurrentMemberList(Context.Guild.Id);
            await ShowMembers(m, sortBy);
            return FromSuccess();
        }

        [Command("Sort", RunMode = RunMode.Async)]
        [Summary("Sort all members by season high.")]
        [Alias("S")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task<RuntimeResult> Sort()
        {
            var m = (await _memberSortService.GetChanges(Context.Guild.Id)).Select(x => x.NewMemberList).ToList();
            await ShowMembers(m);
            return FromSuccess();
        }

        [Command("Changes", RunMode = RunMode.Async)]
        [Summary("Lists the needed changes to clan memberships.")]
        [Alias("C")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task<RuntimeResult> Changes(string compact = null)
        {
            var result = (await _memberSortService.GetChanges(Context.Guild.Id)).Where(x => x.Join.Count > 0 && x.Leave.Count > 0).ToList();

            if(result?.Count() == 0)
                return FromError(CommandError.Unsuccessful, $"No member data in db.");

            var c = await _databaseService.LoadClansAsync();
            var cQty = c?.Count();
            
            if(!string.IsNullOrWhiteSpace(compact))
                await ReplyAsync(GetCompactMemberChangesString(result, c));
            else
            {
                var page = 0;
                var message = await ReplyAsync(GetDetailedMemberChangesString(result, page, c));

                var back = new Emoji("◀️");
                var next = new Emoji("▶️");
                await message.AddReactionsAsync(new IEmote[] { back, next });
                
                await _commandHandler.AddToReactionList(Context.Guild.Id, message, async (r, u) =>
                {
                   if (r.Name == back.Name && page >= 1)
                       await message.ModifyAsync(me => me.Content = GetDetailedMemberChangesString(result, --page, c));
                   else if (r.Name == next.Name && page < cQty-1)
                       await message.ModifyAsync(me => me.Content = GetDetailedMemberChangesString(result, ++page, c));

                   if(u is not null)
                       await message.RemoveReactionAsync(r, u);
                });
            }

            return FromSuccess();
        }

        private async Task ShowMembers(IList<IList<Data.Entities.Member>> mm, SortBy sortBy = SortBy.SHigh)
        {
            var cQty = (await _databaseService.LoadClansAsync(Context.Guild.Id))?.Count();

            if(!cQty.HasValue)
            {
                await ReplyAsync($"No member data in db.");
                return;
            }

            var page = 0;
            var message = await ReplyAsync(GetTable(mm[page], page + 1, sortBy));

            var back = new Emoji("◀️");
            var next = new Emoji("▶️");
            await message.AddReactionsAsync(new IEmote[] { back, next });

            await _commandHandler.AddToReactionList(Context.Guild.Id, message, async (r, u) =>
            {
                if (r.Name == back.Name && page >= 1)
                    await message.ModifyAsync(me => me.Content = GetTable(mm[--page], page + 1, sortBy));
                else if (r.Name == next.Name && page < cQty)
                    await message.ModifyAsync(me => me.Content = GetTable(mm[++page], page + 1, sortBy));

                if(u is not null)
                    await message.RemoveReactionAsync(r, u);
            });
        }

        private string GetCompactMemberChangesString(List<MemberChanges> changes, IList<Data.Entities.Clan> clans)
        {
            var up = new Emoji("↗️");
            var down = new Emoji("↘️");

            var r =  $"```🔄 Sorting List 🔀 {Environment.NewLine}";
            changes.Select((c, i) => new { changes = c, newClan = clans.FirstOrDefault(x => x.SortOrder == (i+1))})
                .SelectMany(x => x.changes.Join, (x, y) => new { Member = y.Member, IsUp = y.IsUp, NewClan = x.newClan, oldClan = y.Member.Clan })
                .OrderBy(x => x.oldClan.SortOrder)
                .GroupBy(x => x.oldClan, (c, m) => m.ToList())
                .ToList()
                .ForEach(x =>
                { 
                    x.ForEach(x => r += $"{x.Member, -24} {(x.IsUp ? up : down)} {x.NewClan}{Environment.NewLine}");
                    r += Environment.NewLine;
                });
            r += "```";

            return r;
        }

        private string GetDetailedMemberChangesString(List<MemberChanges> changes, int index, IList<Data.Entities.Clan> clans)
        {
            var up = new Emoji("⏫");
            var down = new Emoji("⏬");

            var c = clans.FirstOrDefault(x => x.SortOrder == index+1);
            var r =  $"```Incoming changes for {c}: {Environment.NewLine}";

            r += $"Join: {Environment.NewLine}";
            changes[index].Join.ToList().ForEach(x => r += $"{(x.IsUp ? up : down)} {x.Member} - {x.Member.SHigh} {Environment.NewLine}");

            r += Environment.NewLine;

            r += $"Leave: {Environment.NewLine}";
            changes[index].Leave.ToList().ForEach(x => r += /*$"{(x.IsUp ? up : down)}*/ $"{x.Member} - {x.Member.SHigh} {Environment.NewLine}");

            r += "```";

            return r;
        }

        private string GetTable(IList<Data.Entities.Member> members, int? clanNo = null, SortBy sortBy = SortBy.SHigh)
        {
            if(members is null || members.Count <= 0)
                return default;

            switch(sortBy)
            {
                case SortBy.Name:
                    members = members.OrderBy(x => x.Name).ToList();
                    break;

                case SortBy.SHigh:
                    members = members.OrderByDescending(x => x.SHigh).ToList();
                    break;
            }

            var clans = members.Select(x => new { x.Clan.Tag, x.Clan.Name }).Distinct().OrderBy(x => x.Tag).ThenBy(x => x.Name).ToList();

            var table = $"```{Environment.NewLine}";

            if (clanNo.HasValue)
                table += $"{clans?.FirstOrDefault()?.Tag} Members: {members.Count()}{Environment.NewLine}";
                
            table += GetHeader(_header);
            table += GetLimiter(_header);

            foreach (var member in members)
                table += GetValues(member, _fields);

            table += $"{Environment.NewLine}```";
            return table;
        }

        private string GetLimiter(IReadOnlyCollection<string> header)
        {
            var ac = header.Count;

            var l = "";
            for (var i = 0; i < ac; i++)
                l += $"{"-".PadRight(_pad[i], '-')}-";
            l += $"{Environment.NewLine}";
            return l;
        }

        private string GetHeader(IReadOnlyList<string> header)
        {
            var ac = header.Count;

            var l = "";
            for (var i = 0; i < ac; i++)
                l += $"{header[i]?.PadRight(_pad[i])}|";

            l = l.TrimEnd('|');
            l += $"{Environment.NewLine}";
            return l;
        }

        private string GetValues(Data.Entities.Member m, IReadOnlyList<string> header)
        {
            var l = "";
            var ac = header.Count;

            if (m is null)
                return string.Empty;

            for (var i = 0; i < ac; i++)
                l += $"{m.GetPropertyValue(header[i])?.ToString()?.PadRight(_pad[i])}|";

            l = l.TrimEnd('|');
            l += $"{Environment.NewLine}";
            return l;
        }
    }
}
