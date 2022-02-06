using Discord;
using Discord.Commands;
using MMBot.Data.Services.Interfaces;
using MMBot.Discord.Modules.Interfaces;
using MMBot.Discord.Services.Interfaces;
using MMBot.Helpers;

namespace MMBot.Discord.Modules.Member;

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

    [Command("Profile")]
    [Alias("p")]
    [Summary("Shows all information of a member.")]
    public async Task<RuntimeResult> Profile()
    {
        var m = (await _databaseService.LoadMembersAsync(Context.Guild.Id)).FirstOrDefault(x => x.Discord == Context.User.GetUserAndDiscriminator());

        if (m is null)
            return FromErrorObjectNotFound("member", Context.User.Username);

        var imageUrl = await Task.Run(() => Context.Guild.Users.FirstOrDefault(x => x.GetUserAndDiscriminator() == m.Discord)?.GetAvatarUrl());
        var e = m.GetEmbedPropertiesWithValues(imageUrl);
        await ReplyAsync("", false, e as Embed);
        return FromSuccess();
    }
}
