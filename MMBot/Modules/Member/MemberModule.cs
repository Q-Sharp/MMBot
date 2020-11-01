using Discord;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;
using MMBot.Helpers;
using MMBot.Modules.Interfaces;
using MMBot.Services.Interfaces;

namespace MMBot.Modules.Member
{
    [Name("Member")]
    [Group("Member")]
    [Alias("M", "Members")]
    public partial class MemberModule : MMBotModule, IMemberModule
    {
        protected readonly IMemberSortService _memberSortService;

        public MemberModule(IDatabaseService databaseService, ICommandHandler commandHandler, IMemberSortService memberSortService, IGuildSettingsService guildSettings)
            : base(databaseService, guildSettings, commandHandler)
        {
            _memberSortService = memberSortService;
        }

        [Command("Profile", RunMode = RunMode.Async)]
        [Alias("p")]
        [Summary("Shows all information of a member.")]
        public async Task Profile(string name = null)
        {
            var m = name != null
                ? await (await _databaseService.LoadMembersAsync()).FindAndAskForMember(name, Context.Channel, _commandHandler)
                : (await _databaseService.LoadMembersAsync()).FirstOrDefault(x => x.Discord == Context.User.GetUserAndDiscriminator());

            if (m == null)
                return;

            var imageUrl = await Task.Run(() => Context.Guild.Users.FirstOrDefault(x => x.GetUserAndDiscriminator() == m.Discord)?.GetAvatarUrl());
            var e = m.GetEmbedPropertiesWithValues(imageUrl);
            await ReplyAsync("", false, e as Embed);
        }
    }
}
