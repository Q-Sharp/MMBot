using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TTMMBot.Data.Entities;
using TTMMBot.Helpers;
using TTMMBot.Services;
using TTMMBot.Services.Interfaces;

namespace TTMMBot.Modules
{
    [Name("Member")]
    [Group("Member")]
    [Alias("M", "Members")]
    public class MemberModule : ModuleBase<SocketCommandContext>, IMemberModule
    {
        private readonly string[] _header = { "Name", "Clan", "Join", "SHigh", "Role" };
        private readonly int[] _pad = { 16, 4, 4, 5, 7 };
        private readonly string[] _fields = { "Name", "Clan.Tag", "Join", "SHigh", "Role" };

        public IDatabaseService DatabaseService { get; set; }
        public ICommandHandler CommandHandler { get; set; }
        public IGlobalSettingsService GlobalSettings { get; set; }
        public IMemberSortService MemberSortService { get; set; }


        [Command("List", RunMode = RunMode.Async)]
        [Summary("Lists all members by current clan membership.")]
        public async Task List()
        {
            try
            {
                var m = await MemberSortService.GetCurrentMemberList();
                await ShowMembers(m);
            }
            catch (Exception e)
            {
                await ReplyAsync($"{e.Message}");
            }
        }

        [Command("Sort", RunMode = RunMode.Async)]
        [Summary("Sort all members by season high.")]
        [Alias("S")]
        public async Task Sort()
        {
            try
            {
                var m = (await MemberSortService.GetChanges()).Select(x => x.NewMemberList).ToList();
                await ShowMembers(m);
            }
            catch (Exception e)
            {
                await ReplyAsync($"{e.Message}");
            }
        }

        private async Task ShowMembers(IList<IList<Member>> mm)
        {
            var cQty = (await DatabaseService.LoadClansAsync())?.Count();

            if(!cQty.HasValue)
            {
                    await ReplyAsync($"No member data in db.");
                    return;
            }

            var page = 0;
            var message = await ReplyAsync(GetTable(mm[page], page + 1));

            var back = new Emoji("◀️");
            var next = new Emoji("▶️");
            await message.AddReactionsAsync(new IEmote[] { back, next });

            await CommandHandler.AddToReactionList(message, async (r, u) =>
            {
                if (r.Name == back.Name && page >= 1)
                    await message.ModifyAsync(me => me.Content = GetTable(mm[--page], page + 1));
                else if (r.Name == next.Name && page < cQty)
                    await message.ModifyAsync(me => me.Content = GetTable(mm[++page], page + 1));

                if(u != null)
                    await message.RemoveReactionAsync(r, u);
            });
        }

        [Command("Changes", RunMode = RunMode.Async)]
        [Summary("Lists the needed changes to clan memberships.")]
        [Alias("C")]
        public async Task Changes(string compact = null)
        {
            try
            {
                var result = (await MemberSortService.GetChanges()).Where(x => x.Join.Count > 0 && x.Leave.Count > 0).ToList();

                if(result?.Count() == 0)
                {
                    await ReplyAsync($"No member data in db.");
                    return;
                }

                var c = await DatabaseService.LoadClansAsync();
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
                    
                    await CommandHandler.AddToReactionList(message, async (r, u) =>
                    {
                       if (r.Name == back.Name && page >= 1)
                           await message.ModifyAsync(me => me.Content = GetDetailedMemberChangesString(result, --page, c));
                       else if (r.Name == next.Name && page < cQty)
                           await message.ModifyAsync(me => me.Content = GetDetailedMemberChangesString(result, ++page, c));

                       if(u != null)
                           await message.RemoveReactionAsync(r, u);
                    });
                }
            }
            catch (Exception e)
            {
                await ReplyAsync($"{e.Message}");
            }
        }

        private string GetCompactMemberChangesString(List<MemberChanges> changes, IList<Clan> clans)
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

        private string GetDetailedMemberChangesString(List<MemberChanges> changes, int index, IList<Clan> clans)
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

        private string GetTable(IList<Member> members, int? clanNo = null)
        {
            if(members is null || members.Count <= 0)
                return null;

            var table = $"```{Environment.NewLine}";

            if (clanNo.HasValue)
                table += $"[TT{(clanNo.Value == 0 ? string.Empty : clanNo.Value.ToString())}] Members: {members.Count()}{Environment.NewLine}";

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

        private string GetValues(Member m, IReadOnlyList<string> header)
        {
            var l = "";
            var ac = header.Count;

            if (m == null)
                return string.Empty;

            for (var i = 0; i < ac; i++)
                l += $"{m.GetPropertyValue(header[i])?.ToString()?.PadRight(_pad[i])}|";

            l = l.TrimEnd('|');
            l += $"{Environment.NewLine}";
            return l;
        }

        [Command("Show")]
        [Alias("s")]
        [Summary("Shows all information of a member.")]
        public async Task Show(string name = null)
        {
            try
            {
                var m = name != null
                    ? (await DatabaseService.LoadMembersAsync()).FirstOrDefault(x => x.Name == name)
                    : (await DatabaseService.LoadMembersAsync()).FirstOrDefault(x => x.Discord == Context.User.ToString());

                if (m == null)
                {
                    await ReplyAsync($"I can't find {name ?? "you"}!");
                    return;
                }

                var imageUrl = await Task.Run(() => Context.Guild.Users.FirstOrDefault(x => x.GetUserAndDiscriminator() == m.Discord)?.GetAvatarUrl());
                var e = m.GetEmbedPropertiesWithValues(imageUrl);
                await ReplyAsync("", false, e as Embed);

            }
            catch (Exception e)
            {
                await ReplyAsync($"{e.Message}");
            }
        }

        [Command("Delete")]
        [Alias("D")]
        [Summary("Deletes member with given name.")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task Delete(string name)
        {
            try
            {
                var m = (await DatabaseService.LoadMembersAsync()).FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase));
                DatabaseService.DeleteMember(m);
                await DatabaseService.SaveDataAsync();
                await ReplyAsync($"The member {m} was deleted");
            }
            catch (Exception e)
            {
                await ReplyAsync($"{e.Message}");
            }
        }

        [RequireUserPermission(ChannelPermission.ManageRoles)]
        [Command("Set")]
        [Summary("Set [Username] [Property name] [Value]")]
        public async Task Set(string name, string propertyName, [Remainder] string value)
        {
            var m = (await DatabaseService.LoadMembersAsync()).FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase));
            var r = m.ChangeProperty(propertyName, value);
            await DatabaseService.SaveDataAsync();
            await ReplyAsync(r);
        }

        [Command("Create")]
        [Summary("Creates a new member.")]
        public async Task Create(string name)
        {
            try
            {
                var m = await DatabaseService.CreateMemberAsync();
                m.Name = name;
                await DatabaseService.SaveDataAsync();
                await ReplyAsync($"The member {m} was added to database.");
            }
            catch (Exception e)
            {
                await ReplyAsync($"{e.Message}");
            }
        }
    }
}
