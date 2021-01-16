using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using MMBot.Helpers;
using MMBot.Discord.Modules.Interfaces;
using MMBot.Discord.Services.Interfaces;
using MMBot.Data.Services.Interfaces;

namespace MMBot.Discord.Modules.Clan
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
        public async Task<RuntimeResult> List(string tag = null)
        {
            if (tag is null)
            {
                var clans = await _databaseService.LoadClansAsync(Context.Guild.Id);

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
                var c = (await _databaseService.LoadClansAsync(Context.Guild.Id))?.FirstOrDefault(x => string.CompareOrdinal(x.Tag, tag) == 0);

                if(c is null)
                    return FromErrorObjectNotFound("Clan", "tag");
                else
                    await ReplyAsync("", false, c.GetEmbedPropertiesWithValues() as Embed);
            }

            return FromSuccess();
        }

        [RequireUserPermission(ChannelPermission.ManageRoles)]
        [Command("Delete")]
        [Alias("d")]
        [Summary("Deletes clan with given tag.")]
        public async Task<RuntimeResult> Delete(string tag)
        {
            Data.Entities.Clan c;

            try
            {
                c = await (await _databaseService.LoadClansAsync(Context.Guild.Id)).FindAndAskForEntity(Context.Guild.Id, tag, Context.Channel, _commandHandler);
                _databaseService.DeleteClan(c);
                await _databaseService.SaveDataAsync();
            }
            catch(Exception e)
            {
                var s = "data wasn't saved.";
                _logger.LogError(e, s);
                return FromErrorUnsuccessful(s);
            }

            return FromSuccess($"The clan {c} was deleted");
        }

        [RequireUserPermission(ChannelPermission.ManageRoles)]
        [Command("Set")]
        [Summary("Set [Clan tag] [Property name] [Value]")]
        public async Task<RuntimeResult> Set(string tag, string propertyName, [Remainder] string value)
        {
            string m;

            try
            {
                var c = await (await _databaseService.LoadClansAsync(Context.Guild.Id)).FindAndAskForEntity(Context.Guild.Id, tag, Context.Channel, _commandHandler);
                m = c.ChangeProperty(propertyName, value);
                await _databaseService.SaveDataAsync();
            }
            catch(Exception e)
            {
                var s = "data wasn't saved.";
                _logger.LogError(e, s);
                return FromErrorUnsuccessful(s);
            }

            return FromSuccess(m);
        }

        [RequireUserPermission(ChannelPermission.ManageRoles)]
        [Command("Create")]
        [Summary("Creates a new clan")]
        public async Task<RuntimeResult> Create(string tag, [Remainder] string name)
        {
            Data.Entities.Clan c;
            var gs = await _databaseService.LoadClansAsync(Context.Guild.Id);

            try
            {
                c = await _databaseService.CreateClanAsync(Context.Guild.Id);
                c.Tag = tag;
                c.Name = name;
                c.SortOrder = gs.Any() ? gs.Max(y => y.SortOrder) + 1 : 1;
                await _databaseService.SaveDataAsync();
            }
            catch(Exception e)
            {
                var s = "data wasn't saved.";
                _logger.LogError(e, s);
                return FromErrorUnsuccessful(s);
            }

            return FromSuccess($"The clan {c} was added to database.");
        }

        [RequireUserPermission(ChannelPermission.ManageRoles)]
        [Command("AddMember")]
        [Summary("Adds a member with name to clan with tag")]
        public async Task<RuntimeResult> AddMember(string tag, [Remainder] string memberName)
        {
            var cs = await _databaseService.LoadClansAsync(Context.Guild.Id);
            var ms = await _databaseService.LoadMembersAsync(Context.Guild.Id);

            var c = await cs.FindAndAskForEntity(Context.Guild.Id, tag, Context.Channel, _commandHandler);
            var m = await ms.FindAndAskForEntity(Context.Guild.Id, memberName, Context.Channel, _commandHandler);

            if (m is not null && c is not null)
            {
                m.ClanId = c.Id;

                await _databaseService.SaveDataAsync();
                await ReplyAsync($"The member {m} is now member of {c}.");
                return FromSuccess();
            }

            if(m is null)
                return FromErrorObjectNotFound("Member", memberName);
            else
                return FromErrorObjectNotFound("Clan", tag);
        }
    }
}
