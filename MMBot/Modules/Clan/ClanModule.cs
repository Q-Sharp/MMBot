using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using MMBot.Helpers;
using MMBot.Modules.Interfaces;
using MMBot.Services.Interfaces;

namespace MMBot.Modules.Clan
{
    [Name("Clan")]
    [Group("Clan")]
    [Alias("C", "Clans")]
    public class ClanModule : MMBotModule, IClanModule
    {
        public ClanModule(IDatabaseService databaseService, ILogger<ClanModule> logger, IGuildSettingsService guildSettings, ICommandHandler commandHandler)
            : base(databaseService, guildSettings, commandHandler)
        {
        }

        [Command("List")]
        [Summary("Lists all Clans")]
        public async Task<RuntimeResult> List(string tag = null)
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
            var c = (await _databaseService.GetClanAsync(tag, Context.Guild.Id));
            _databaseService.DeleteClan(c);
            await _databaseService.SaveDataAsync();
            await ReplyAsync($"The clan {c} was deleted");
            return FromSuccess();
        }

        [RequireUserPermission(ChannelPermission.ManageRoles)]
        [Command("Set")]
        [Summary("Set [Clan tag] [Property name] [Value]")]
        public async Task<RuntimeResult> SetCommand(string tag, string propertyName, [Remainder] string value)
        {
            var c = (await _databaseService.LoadClansAsync()).FirstOrDefault(x => x.Tag == tag);
            var m = c.ChangeProperty(propertyName, value);
            await _databaseService.SaveDataAsync();
            await ReplyAsync(m);
            return FromSuccess();
        }

        [RequireUserPermission(ChannelPermission.ManageRoles)]
        [Command("Create")]
        [Summary("Creates a new clan")]
        public async Task<RuntimeResult> Create(string tag, [Remainder] string name)
        {
            var c = await _databaseService.CreateClanAsync(Context.Guild.Id);
            c.Tag = tag;
            c.Name = name;
            await _databaseService.SaveDataAsync();
            await ReplyAsync($"The clan {c} was added to database.");
            return FromSuccess();
        }

        [RequireUserPermission(ChannelPermission.ManageRoles)]
        [Command("AddMember")]
        [Summary("Adds a member with name to clan with tag")]
        public async Task<RuntimeResult> AddMember(string tag, string memberName)
        {
            var c = (await _databaseService.LoadClansAsync()).FirstOrDefault(x => x.Tag == tag);
            var m = (await _databaseService.LoadMembersAsync()).FirstOrDefault(x => x.Name == memberName);

            if (m is not null || c is not null)
            {
                m.Clan.Tag = c.Tag;

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
