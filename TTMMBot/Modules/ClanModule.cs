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
    [Name("Clan")]
    [Group("Clan")]
    [Alias("clan", "c", "C", "clans", "Clans")]
    [RequireUserPermission(ChannelPermission.ManageRoles)]
    public class ClanModule : ModuleBase<SocketCommandContext>
    {
        public IDatabaseService DatabaseService { get; set; }

        [Command]
        [Summary("Lists all Clans")]
        public async Task Clan()
        {
            try
            {
                var clans = await DatabaseService.LoadClansAsync();

                var builder = new EmbedBuilder
                {
                    Color = Color.DarkTeal,
                    Description = "Clans",
                    Title = "Clans"
                };

                foreach (var clan in clans)
                {
                    builder.AddField(x =>
                    {
                        x.Name = clan.Tag;
                        x.Value = clan.Name;
                        x.IsInline = false;
                    });
                }

                await ReplyAsync("", false, builder.Build());
            }
            catch (Exception e)
            {
                await ReplyAsync($"{e.Message}");
            }
        }

        

        [Command("Delete")]
        [Alias("delete", "d", "D")]
        [Summary("Deletes clan with given tag.")]
        public async Task Delete(string tag)
        {
            try
            {
                var c = (await DatabaseService.LoadClansAsync()).FirstOrDefault(x => x.Tag == tag);
                DatabaseService.DeleteClan(c);
                await DatabaseService.SaveDataAsync();
                await ReplyAsync($"The clan {c} was deleted");
            }
            catch (Exception e)
            {
                await ReplyAsync($"{e.Message}");
            }
        }

        [Group("Set")]
        [Alias("set", "s", "S")]
        [Summary("Change...")]
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
            [Summary("...a new clan")]
            public async Task Clan(string tag, [Remainder] string name)
            {
                try
                {
                    var c = await DatabaseService.CreateClanAsync();
                    c.Tag = tag;
                    c.Name = name;
                    await DatabaseService.SaveDataAsync();
                    await ReplyAsync($"The clan {c} was added to database.");
                }
                catch (Exception e)
                {
                    await ReplyAsync($"{e.Message}");
                }
            }

            [Command("Member")]
            [Alias("member", "m", "m")]
            [Summary(".... to clan with given tag, member with given membername")]
            public async Task Member(string tag, string memberName)
            {
                try
                {
                    var c = (await DatabaseService.LoadClansAsync()).FirstOrDefault(x => x.Tag == tag);
                    var m = (await DatabaseService.LoadMembersAsync()).FirstOrDefault(x => x.Name == memberName);

                    m.Clan.Tag = c.Tag;

                    await DatabaseService.SaveDataAsync();
                    await ReplyAsync($"The member {m} is now member of {c}.");
                }
                catch (Exception e)
                {
                    await ReplyAsync($"{e.Message}");
                }
            }
        }
    }
}
