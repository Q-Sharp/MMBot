using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using TTMMBot.Data.Enums;
using TTMMBot.Services;
using static TTMMBot.Data.Entities.SetEntityPropertiesHelper;

namespace TTMMBot.Modules
{
    [Name("Clan")]
    [Group("Clan")]
    [Alias("C", "Clans")]
    [RequireUserPermission(ChannelPermission.ManageRoles)]
    public class ClanModule : ModuleBase<SocketCommandContext>
    {
        public ILogger<ClanModule> Logger { get; set; }

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

        [RequireUserPermission(ChannelPermission.ManageRoles)]
        [Command("FixRoles", RunMode = RunMode.Async)]
        [Alias("FR")]
        [Summary("Checks and fixes discord roles of all clan members.")]
        public async Task FixRoles() =>
            await Task.Run(async () =>
            {
                try
                {
                    var c = await DatabaseService.LoadClansAsync();
                    var ar = Context.Guild.Roles.Where(x => c.Select(clan => clan.DiscordRole).Contains(x.Name)).ToArray();

                    foreach (var clan in c)
                    {
                        var clanRole = Context.Guild.Roles.FirstOrDefault(x => x.Name == clan.DiscordRole) as IRole;

                        foreach (var member in clan.Member)
                        {
                            var user = await Task.Run(() => Context.Guild.Users.FirstOrDefault(x => $"{x.Username}#{x.Discriminator}" == member.Discord));

                            if (user is null || member.ClanID is null || clan.DiscordRole is null)
                                continue;

                            try
                            {
                                if (member.Role == Role.CoLeader || member.Role == Role.Leader)
                                {
                                    await user.RemoveRolesAsync(ar);
                                    await user.AddRolesAsync(ar);
                                }
                                else
                                {
                                    await user.RemoveRolesAsync(ar);
                                    await user.AddRoleAsync(clanRole);
                                }
                            }
                            catch (Exception e)
                            {
                                await ReplyAsync($"{member.Name}'s role couldn't be fixed: {e.Message}");
                            }
                        }
                    }

                    await ReplyAsync($"All roles have been fixed!").ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    await ReplyAsync($"{e.Message}");
                }
            });

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

        [Group("Set")]
        [Alias("S")]
        [Summary("Change...")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public class Set : ModuleBase<SocketCommandContext>
        {
            public IDatabaseService DatabaseService { get; set; }
            public Set(IDatabaseService databaseService) => DatabaseService = databaseService;

            [Command]
            [Summary(".... Name")]
            public async Task SetCommand(string propertyName, string tag, [Remainder] string newName)
            {
                var c = (await DatabaseService.LoadClansAsync()).FirstOrDefault(x => x.Tag == tag);
                await ChangeProperty(c, propertyName, newName, x => ReplyAsync(x));
                await DatabaseService.SaveDataAsync();
            }
        }

        [Group("Add")]
        [Alias("A")]
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
            [Alias("M")]
            [Summary(".... to clan with given tag, member with given member name")]
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
