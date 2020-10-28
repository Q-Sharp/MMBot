using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MMBot.Helpers;
using MMBot.Modules.Interfaces;
using MMBot.Services;
using MMBot.Services.Interfaces;

namespace MMBot.Modules.Clan
{
    [Name("Clan")]
    [Group("Clan")]
    [Alias("C", "Clans")]
    public class ClanModule : MMBotModule, IClanModule
    {
        private ILogger<ClanModule> _logger;

        public ClanModule(IDatabaseService databaseService, ILogger<ClanModule> logger, IGuildSettingsService guildSettings, ICommandHandler commandHandler)
            : base(databaseService, guildSettings, commandHandler)
        {
            _logger = logger;
        }

        [Command("List")]
        [Summary("Lists all Clans")]
        public async Task List(string tag = null)
        {
            try
            {
                if (tag is null)
                {

                    var clans = await _databaseService.LoadClansAsync();

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
                    var c = (await _databaseService.LoadClansAsync())?.FirstOrDefault(x => string.CompareOrdinal(x.Tag, tag) == 0);

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
                var c = (await _databaseService.LoadClansAsync()).FirstOrDefault(x => x.Tag == tag);
                _databaseService.DeleteClan(c);
                await _databaseService.SaveDataAsync();
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
            var c = (await _databaseService.LoadClansAsync()).FirstOrDefault(x => x.Tag == tag);
            var m = c.ChangeProperty(propertyName, value);
            await _databaseService.SaveDataAsync();
            await ReplyAsync(m);
        }

        [RequireUserPermission(ChannelPermission.ManageRoles)]
        [Command("Create")]
        [Summary("Creates a new clan")]
        public async Task Create(string tag, [Remainder] string name)
        {
            try
            {
                var c = await _databaseService.CreateClanAsync();
                c.Tag = tag;
                c.Name = name;
                await _databaseService.SaveDataAsync();
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
                var c = (await _databaseService.LoadClansAsync()).FirstOrDefault(x => x.Tag == tag);
                var m = (await _databaseService.LoadMembersAsync()).FirstOrDefault(x => x.Name == memberName);

                if (m != null && c != null)
                {
                    m.Clan.Tag = c.Tag;

                    await _databaseService.SaveDataAsync();
                    await ReplyAsync($"The member {m} is now member of {c}.");
                }
            }
            catch (Exception e)
            {
                await ReplyAsync($"{e.Message}");
            }
        }
    }
}
