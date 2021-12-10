using System;
using System.Linq;
using System.Text.RegularExpressions;
using Discord;

namespace MMBot.Helpers
{
    public static class DiscordHelpers
    {
        public static string GetUserAndDiscriminator(this IUser sgu) => $"{sgu.Username}#{sgu.Discriminator}";

        public static string PrepareForTranslate(this string text)
        {
            var newtext = Regex.Replace(text, @"(<[:@].+?>)|`|(\u00a9|\u00ae|[\u2000-\u3300]|\ud83c[\ud000-\udfff]|\ud83d[\ud000-\udfff]|\ud83e[\ud000-\udfff])", 
                string.Empty, RegexOptions.Compiled);

            return Regex.Replace(newtext.Trim(), @"([^\p{L}0-9_.!\s?\?])+", string.Empty, RegexOptions.Compiled).Trim();
        }
    }
}
