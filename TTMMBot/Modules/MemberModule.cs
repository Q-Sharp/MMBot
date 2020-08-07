using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using TTMMBot.Services;

namespace TTMMBot.Modules
{
    [Name("Member")]
    [Group("Member")]
    [Alias("member", "m", "M", "Members", "members")]
    public class MemberModule : ModuleBase<SocketCommandContext>
    {
        public IDatabaseService DatabaseService { get; set; }

        [Command]
        [Summary("Shows all information of member")]
        public async Task Member(string name = null)
        {
            try
            {
                var m = name != null ? (await DatabaseService.LoadMembersAsync()).FirstOrDefault(x => x.Name == name)
                    : (await DatabaseService.LoadMembersAsync()).FirstOrDefault(x => x.Discord == Context.User.ToString());

                if(m == null)
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
