using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using TTMMBot.Data.Entities;
using TTMMBot.Helpers;
using TTMMBot.Services;

namespace TTMMBot.Modules
{
    [Name("Member")]
    [Group("Member")]
    [Alias("member", "m", "M", "Members", "members")]
    public class MemberModule : ModuleBase<SocketCommandContext>
    {
        private readonly string[] header = { "Name", "Clan", "AHigh", "SHigh", "Role" };
        private readonly int[] pad = { 16, 4, 5, 5, 7 };
        private readonly string[] fields = { "Name", "Clan.Tag", "AllTimeHigh", "SeasonHighest", "Role" };

        public IDatabaseService DatabaseService { get; set; }
        public CommandHandler CommandHandler { get; set; }

        [Command]
        [Summary("Lists all members")]
        public async Task Member()
        {
            try
            {
                var m = await DatabaseService.LoadMembersAsync();

                foreach (var gM in m.Where(x => x.IsActive)
                    .GroupBy(x => x.ClanID, (x, y) => new { Clan = x, Members = y })
                    .OrderBy(x => x.Clan))
                {
                    if (gM.Clan.HasValue && (gM?.Members?.FirstOrDefault()?.Clan?.Tag != null))
                    {
                        var table = GetTable(gM.Members.ToList());
                        await ReplyAsync(table);
                    }
                }
            }
            catch (Exception e)
            {
                await ReplyAsync($"{e.Message}");
            }
        }

        [Command("Sort")]
        [Summary("Lists all members")]
        [Alias("sort", "s", "S")]
        public async Task Sort()
        {
            try
            {
                var m = (await DatabaseService.LoadMembersAsync()).Where(x => x.IsActive).ToList();

                int page = 1;
                

                var message = await ReplyAsync(GetSortedMembersTable(m, page));

                var back = new Emoji("◀️");
                var next = new Emoji("▶️");
                await message.AddReactionsAsync(new Emoji[] { back, next });

                CommandHandler.AddToReactionList(message, async r =>
                {
                    if (r.Name == back.Name && page > 1)
                        await message.ModifyAsync(async me => me.Content = await Task.Run(() => GetSortedMembersTable(m, page)));
                    else if (r.Name == next.Name && page < 5)
                        await message.ModifyAsync(async me => me.Content = await Task.Run(() => GetSortedMembersTable(m, page)));
                });
            }
            catch (Exception e)
            {
                await ReplyAsync($"{e.Message}");
            }
        }

        [Command("Changes")]
        [Summary("Lists member changes")]
        [Alias("changes", "c", "C")]
        public async Task Changes()
        {
            try
            {
                var m = (await DatabaseService.LoadMembersAsync()).Where(x => x.IsActive).ToList();

                int page = 1;
                var table = await Task.Run(() => GetSortedMembersTable(m, page));

                var message = await ReplyAsync(table);

                var back = new Emoji("◀️");
                var next = new Emoji("▶️");
                await message.AddReactionsAsync(new Emoji[] { back, next });

                CommandHandler.AddToReactionList(message, async r =>
                {
                    if (r.Name == back.Name && page > 1)
                        await message.ModifyAsync(async me => me.Content = await Task.Run(() => GetSortedMembersTable(m, page)));
                    else if (r.Name == next.Name && page < 5)
                        await message.ModifyAsync(async me => me.Content = await Task.Run(() => GetSortedMembersTable(m, page)));
                });
            }
            catch (Exception e)
            {
                await ReplyAsync($"{e.Message}");
            }
        }

        private IList<Member> GetSortedMembers(IList<Member> m, int pageNo) => m.OrderByDescending(x => x.TotalScore).ToList().ChunkBy(20)[pageNo - 1].ToList();

        private string GetSortedMembersTable(IList<Member> m, int pageNo)
        {
            var sortedm = GetSortedMembers(m, pageNo);
            return GetTable(sortedm);
        }

        private string GetTable(IList<Member> members, int? clanNo = null)
        {
            var table = $"```{Environment.NewLine}";

            if (clanNo.HasValue)
                table += $"[TT {clanNo.Value}]{Environment.NewLine}";

            table += getHeader(header);
            table += getLimiter(header);

            foreach (var member in members.OrderByDescending(x => x.AllTimeHigh))
                table += getValues(member, fields);

            table += $"{Environment.NewLine}```";

            return table;
        }

        private string getLimiter(string[] header)
        {
            var ac = header.Length;

            var l = "";
            for (var i = 0; i < ac; i++)
                l += $"{"-"?.PadRight(pad[i], '-')}-";
            l += $"{Environment.NewLine}";
            return l;
        }

        private string getHeader(string[] header)
        {
            var ac = header.Length;

            var l = "";
            for (var i = 0; i < ac; i++)
                l += $"{header[i]?.PadRight(pad[i])}|";

            l = l.TrimEnd('|');
            l += $"{Environment.NewLine}";
            return l;
        }

        private string getValues(Member m, string[] header)
        {
            var l = "";
            var ac = header.Length;

            if (m == null || header == null)
                return string.Empty;

            for (var i = 0; i < ac; i++)
                l += $"{m.GetPropertyValue(header[i])?.ToString()?.PadRight(pad[i])}|";

            l = l.TrimEnd('|');
            l += $"{Environment.NewLine}";
            return l;
        }

        [Command("Me")]
        [Summary("Shows all information of member")]
        public async Task Member(string name = null)
        {
            try
            {
                var m = name != null ? (await DatabaseService.LoadMembersAsync()).FirstOrDefault(x => x.Name == name)
                    : (await DatabaseService.LoadMembersAsync()).FirstOrDefault(x => x.Discord == Context.User.ToString());

                if (m == null)
                {
                    await ReplyAsync($"I can't find {name ?? "you"}!");
                    return;
                }

                var builder = new EmbedBuilder
                {
                    Color = Color.DarkTeal,
                    Description = m.Name,
                };

                builder.WithTitle(m?.Clan?.Tag);

                builder.AddField(x =>
                {
                    x.Name = "Name";
                    x.Value = m;
                    x.IsInline = true;
                });

                await ReplyAsync("", false, builder.Build());

            }
            catch (Exception e)
            {
                await ReplyAsync($"{e.Message}");
            }
        }

        [Command("Delete")]
        [Alias("delete", "d", "D")]
        [Summary("Deletes member with given name.")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task Delete(string name)
        {
            try
            {
                var m = (await DatabaseService.LoadMembersAsync()).FirstOrDefault(x => x.Name == name);
                DatabaseService.DeleteMember(m);
                await DatabaseService.SaveDataAsync();
                await ReplyAsync($"The member {m} was deleted");
            }
            catch (Exception e)
            {
                await ReplyAsync($"{e.Message}");
            }
        }

        [Group("Set")]
        [Alias("set", "s", "S")]
        [Summary("Change...")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public class Set : ModuleBase<SocketCommandContext>
        {
            public IDatabaseService DatabaseService { get; set; }

            public Set(IDatabaseService databaseService) => DatabaseService = databaseService;

            [Command("Tag")]
            [Alias("tag", "t", "T")]
            [Summary("... Tag")]
            public async Task Tag(string tag, string newTag)
            {
                var c = (await DatabaseService.LoadClansAsync()).FirstOrDefault(x => x.Tag == tag);
                c.Tag = newTag;
                await DatabaseService.SaveDataAsync();
                await ReplyAsync($"The {c} now uses {c.Tag} instead of {tag}.");
            }

            [Command("Name")]
            [Alias("name", "n", "N")]
            [Summary(".... Name")]
            public async Task Name(string tag, [Remainder] string newName)
            {
                var c = (await DatabaseService.LoadClansAsync()).FirstOrDefault(x => x.Tag == tag);
                var oldName = c.Name;
                c.Name = newName;
                await DatabaseService.SaveDataAsync();
                await ReplyAsync($"The {c} now uses {c.Name} instead of {oldName}.");
            }
        }

        [Group("Add")]
        [Alias("add", "a", "A")]
        [Summary("Add....")]
        public class Add : ModuleBase<SocketCommandContext>
        {
            public IDatabaseService DatabaseService { get; set; }

            [Command]
            [Summary("...a new member")]
            public async Task Member(string name)
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

            //[Command("Member")]
            //[Alias("member", "m", "m")]
            //[Summary(".... to clan with given tag, member with given membername")]
            //public async Task Member(string tag, string memberName)
            //{
            //    try
            //    {
            //        var c = (await DatabaseService.LoadClansAsync()).FirstOrDefault(x => x.Tag == tag);
            //        var m = (await DatabaseService.LoadMembersAsync()).FirstOrDefault(x => x.Name == memberName);

            //        m.ClanTag = c.Tag;

            //        await DatabaseService.SaveDataAsync();
            //        await ReplyAsync($"The member {m} is now member of {c}.");
            //    }
            //    catch (Exception e)
            //    {
            //        await ReplyAsync($"{e.Message}");
            //    }
            //}
        }
    }
}
