using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using TTMMBot.Data.Entities;
using TTMMBot.Data.Enums;
using TTMMBot.Helpers;
using TTMMBot.Modules.Enums;
using TTMMBot.Services;

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

        [Command("List", RunMode = RunMode.Async)]
        [Summary("Lists all members by current clan membership.")]
        public async Task List()
        {
            try
            {
                var m = (await DatabaseService.LoadMembersAsync()).Where(x => x.IsActive).ToList();
                await ShowMembers(m, SortType.ByClan);
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
                var m = (await DatabaseService.LoadMembersAsync()).Where(x => x.IsActive).ToList();
                await ShowMembers(m, SortType.BySeasonHigh);
            }
            catch (Exception e)
            {
                await ReplyAsync($"{e.Message}");
            }
        }

        private async Task ShowMembers(IList<Member> m, SortType sortedBy)
        {
            int? chunkSize = null;

            switch(sortedBy)
            {
                case SortType.ByClan:
                    m = m.OrderBy(x => x.Clan?.Tag).ToList();
                    break;

                case SortType.BySeasonHigh:
                    m = m.OrderByDescending(x => x.SHigh).ToList();
                    chunkSize = GlobalSettings.ClanSize;
                    break;
            }

            var page = 1;
            var message = await ReplyAsync(GetSortedMembersTable(m, page, chunkSize));

            var back = new Emoji("◀️");
            var next = new Emoji("▶️");
            await message.AddReactionsAsync(new IEmote[] { back, next });

            await CommandHandler.AddToReactionList(message, async r =>
            {
                if (r.Name == back.Name && page > 1)
                    await message.ModifyAsync(me => me.Content = GetSortedMembersTable(m, --page, chunkSize));
                else if (r.Name == next.Name && page < 5)
                    await message.ModifyAsync(me => me.Content = GetSortedMembersTable(m, ++page, chunkSize));
            });
        }

        [Command("Changes")]
        [Summary("Lists the needed changes to clan memberships.")]
        [Alias("C")]
        public async Task Changes()
        {
            try
            {
                var m = (await DatabaseService.LoadMembersAsync()).Where(x => x.IsActive).ToList();

                var current = m.OrderBy(x => x.Clan?.Tag)
                    .GroupBy(x => x.ClanId, (x, y) => new { Clan = x, Members = y })
                    .Select(x => x.Members.ToList() as IList<Member>)
                    .Select(x => x.OrderByDescending(y => y.SHigh).ToList())
                    .ToList();

                var future = m.OrderByDescending(x => x.SHigh)
                    .ToList()
                    .ChunkBy(20);

                if (current is null || future is null)
                    return;


                int moveQty = GlobalSettings.MemberMovementQty;

                var highLowGroup = current
                    .Select((x, i) => new 
                    { 
                        ClanId = x.FirstOrDefault().ClanId, 
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
                    .Select(x => new
                    {
                        x.ClanId,
                        Leave = x.Curlow.Where(y => x.result != null && !x.result.Contains(y)).ToList(),
                        Join = x.Nexthigh.Where(y => x.result.Contains(y)).ToList()
                    }).ToList();


                var up = new Emoji("⏫");
                var down = new Emoji("⏬");

                var c = (await DatabaseService.LoadClansAsync()).ToList();
                var r2 = $"Exchange these member: {Environment.NewLine}";
                r2 += Environment.NewLine;
                foreach(var re in result.Where(x => x.Join.Count > 0 && x.Leave.Count > 0))
                { 
                    r2 += Environment.NewLine;
                    var cc = c.FirstOrDefault(x => x.ClanId == re.ClanId);

                    r2 +=  $"Move to {cc}: {Environment.NewLine}";
                    re.Join.ForEach(x => r2 += $"{up} {x} - {x.SHigh} {Environment.NewLine}");
                    r2 += Environment.NewLine;
                    r2 +=  $"Remove from {cc}: {Environment.NewLine}";
                    re.Leave.ForEach(x => r2 += $"{down} {x} - {x.SHigh} {Environment.NewLine}");
                    r2 += Environment.NewLine;
                }
                await ReplyAsync(r2);


                //var r = $"Exchange these member: {Environment.NewLine}";
                //for (var i = 0; i < current.Count(); i++)
                //{
                //    var dif = future[i].Where(x => !x.IgnoreOnMoveUp && x.Role < Role.CoLeader && !current[i].Contains(x))
                //        .OrderByDescending(x => x.SHigh)
                //        .ToList();
                    
                //    r += GetTable(dif, i+1);
                //    r += Environment.NewLine;
                //}

                //await ReplyAsync(r);
            }
            catch (Exception e)
            {
                await ReplyAsync($"{e.Message}");
            }
        }

        private string GetSortedMembersTable(IList<Member> m, int pageNo, int? chunkSize = null)
        {
            IList<Member> sorted;

            if(!chunkSize.HasValue)
            {
                sorted = m.GroupBy(x => x.ClanId, (x, y) => new { Clan = x, Members = y })
                    .OrderBy(x => x.Clan)
                    .Select(x => x.Members.Select(v => v).ToList() as IList<Member>)
                    .ToArray()[pageNo - 1]
                    .ToList();
            }
            else
                sorted = m.ChunkBy(chunkSize.Value)[pageNo - 1].ToList();

            return GetTable(sorted, pageNo);
        }

        private string GetTable(IList<Member> members, int? clanNo = null)
        {
            if(members is null || members.Count <= 0)
                return null;

            var table = $"```{Environment.NewLine}";

            if (clanNo.HasValue)
                table += $"[TT{clanNo.Value}] Members: {members.Count()}{Environment.NewLine}";

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
