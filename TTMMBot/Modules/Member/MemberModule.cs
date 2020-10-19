using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;
using TTMMBot.Helpers;
using TTMMBot.Modules.Interfaces;
using TTMMBot.Services.Interfaces;

namespace TTMMBot.Modules.Member
{
    [Name("Member")]
    [Group("Member")]
    [Alias("M", "Members")]
    public partial class MemberModule : ModuleBase<SocketCommandContext>, IMemberModule
    {
        private readonly IDatabaseService _databaseService;
        private readonly ICommandHandler _commandHandler;
        private readonly IGlobalSettingsService _globalSettings;
        private readonly IMemberSortService _memberSortService;

        public MemberModule(IDatabaseService databaseService, ICommandHandler commandHandler, IGlobalSettingsService globalSettingsService, IMemberSortService memberSortService)
        {
            _databaseService = databaseService;
            _commandHandler = commandHandler;
            _globalSettings = globalSettingsService;
            _memberSortService = memberSortService;
        }

        [Command("Profile")]
        [Alias("p")]
        [Summary("Shows all information of a member.")]
        public async Task Profile(string name = null)
        {
            try
            {
                var m = name != null
                    ? (await _databaseService.LoadMembersAsync()).FirstOrDefault(x => string.CompareOrdinal(x.Name, name) == 0)
                    : (await _databaseService.LoadMembersAsync()).FirstOrDefault(x => x.Discord == Context.User.GetUserAndDiscriminator());

                if (m == null)
                {
                    await ReplyAsync($"I can't find {name ?? "you"}!");
                    return;
                }

                var imageUrl = await Task.Run(() => Context.Guild.Users.FirstOrDefault(x => x.GetUserAndDiscriminator() == m.Discord)?.GetAvatarUrl());
                var e = m.GetEmbedPropertiesWithValues(imageUrl);
                await ReplyAsync("", false, e as Embed);

            }
            catch (Exception e)
            {
                await ReplyAsync($"{e.Message}");
            }
        }
    }
}
