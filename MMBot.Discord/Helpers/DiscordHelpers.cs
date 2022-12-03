using System.Text.RegularExpressions;
using Discord;
using Discord.Commands;

namespace MMBot.Discord.Helpers;

public static partial class DiscordHelpers
{
    public static string GetUserAndDiscriminator(this IUser sgu) => $"{sgu.Username}#{sgu.Discriminator}";

    public static string PrepareForTranslate(this string text)
    {
        var newtext = Step1RegEx().Replace(text, string.Empty);

        return Step2RegEx().Replace(newtext.Trim(), string.Empty).Trim();
    }

    public static bool IsAdmin(this SocketCommandContext ctx)
    {
        var roles = ctx.Guild.Roles.Where(x => x.Members.Select(x => x.Id).Contains(ctx.User.Id));
        return roles.Any(x => x.Permissions.Administrator) || ctx.User.Id == ctx.Guild.OwnerId;
    }

    public static bool IsOwner(this IUser sgu) => sgu.Id == 301764235887902727;

    [GeneratedRegex("([^\\p{L}0-9_'.!\\s?\\?])+", RegexOptions.Compiled)]
    private static partial Regex Step2RegEx();

    [GeneratedRegex("(<[:@].+?>)|`|(\\u00a9|\\u00ae|[\\u2000-\\u3300]|\\ud83c[\\ud000-\\udfff]|\\ud83d[\\ud000-\\udfff]|\\ud83e[\\ud000-\\udfff])", RegexOptions.Compiled)]
    private static partial Regex Step1RegEx();
}
