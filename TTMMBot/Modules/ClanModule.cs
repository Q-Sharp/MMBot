using System;
using System.Linq;
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
        [Command("FixRoles")]
        [Alias("FR")]
        [Summary("Checks and fixes discord roles of all clan members.")]
        public async Task FixRoles() =>
            await Task.Run(async () =>
            {
                try
                {
                    var c = await DatabaseService.LoadClansAsync();
                    var ar = Context.Guild.Roles.ToAsyncEnumerable()
                        .Where(x => c.Select(x => x.DiscordRole).Contains(x.Name)).ToArrayAsync();

                    await foreach (var clan in c.ToAsyncEnumerable())
                    {
                        var clanRole = await Context.Guild.Roles.ToAsyncEnumerable()
                            .FirstOrDefaultAsync(x => x.Name == clan.DiscordRole) as IRole;

                        await foreach (var member in clan.Member.ToAsyncEnumerable())
                        {
                            var user = await Task.Run(() =>
                                Context.Guild.Users.FirstOrDefault(x =>
                                    $"{x.Username}#{x.Discriminator}" == member.Discord)).ConfigureAwait(false);

                            if (user is null || member.ClanID is null || clan.DiscordRole is null)
                                continue;

                            try
                            {
                                if (member.Role == Role.CoLeader || member.Role == Role.Leader)
                                {
                                    await user.RemoveRolesAsync(await ar).ConfigureAwait(false);
                                    await user.AddRolesAsync(await ar).ConfigureAwait(false);
                                }
                                else
                                {
                                    await user.RemoveRolesAsync(await ar).ConfigureAwait(false);
                                    await user.AddRoleAsync(clanRole).ConfigureAwait(false);
                                }
                            }
                            catch (Exception e)
                            {
                                await ReplyAsync($"{member.Name}'s role couldn't be fixed: {e.Message}")
                                    .ConfigureAwait(false);
                            }
                        }
                    }

                    await ReplyAsync($"All roles have been fixed!").ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    await ReplyAsync($"{e.Message}").ConfigureAwait(false);
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
