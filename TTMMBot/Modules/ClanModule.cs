using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using TTMMBot.Data.Enums;
using TTMMBot.Services;
using static TTMMBot.Data.Entities.EntityHelpers;

namespace TTMMBot.Modules
{
    [Name("Clan")]
    [Group("Clan")]
    [Alias("C", "Clans")]
    public class ClanModule : ModuleBase<SocketCommandContext>
    {
        public ILogger<ClanModule> Logger { get; set; }

        public IDatabaseService DatabaseService { get; set; }

        [Command("List")]
        [Summary("Lists all Clans")]
        public async Task List(string tag = null)
        {
            try
            {
                if (tag is null)
                {

                    var clans = await DatabaseService.LoadClansAsync();

                    var builder = new EmbedBuilder { Color = Color.DarkTeal, Title = "Clans" };

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
                else
                {
                    var c = (await DatabaseService.LoadClansAsync())?.FirstOrDefault(x => string.CompareOrdinal(x.Tag, tag) == 0);

                    if(c == null)
                        await ReplyAsync("I don't know this clan.");
                    else
                        await ReplyAsync("", false, c.GetEmbedPropertiesWithValues() as Embed);
                }
            }
            catch (Exception e)
            {
                await ReplyAsync($"{e.Message}");
            }
        }

        [RequireUserPermission(ChannelPermission.ManageRoles)]
        [Command("Delete")]
        [Alias("d")]
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

        [RequireUserPermission(ChannelPermission.ManageRoles)]
        [Command("Set")]
        [Summary("Set [Clan tag] [Property name] [Value]")]
        public async Task SetCommand(string tag, string propertyName, [Remainder] string value)
        {
            var c = (await DatabaseService.LoadClansAsync()).FirstOrDefault(x => x.Tag == tag);
            var m = c.ChangeProperty(propertyName, value);
            await DatabaseService.SaveDataAsync();
            await ReplyAsync(m);
        }

        [RequireUserPermission(ChannelPermission.ManageRoles)]
        [Command("Create")]
        [Summary("Creates a new clan")]
        public async Task Create(string tag, [Remainder] string name)
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

        [RequireUserPermission(ChannelPermission.ManageRoles)]
        [Command("AddMember")]
        [Summary("Adds a member with name to clan with tag")]
        public async Task AddMember(string tag, string memberName)
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
