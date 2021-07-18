using Discord;

namespace MMBot.Helpers
{
    public static class DiscordHelpers
    {
        public static string GetUserAndDiscriminator(this IUser sgu) => $"{sgu.Username}#{sgu.Discriminator}";

        public static bool HasMention(string text) 
            => !string.IsNullOrEmpty(text) && text.Length > 3 && text.Contains('<') && text.Contains('@') && text.Contains('>');

        public static string SeperateMention(string text, out string mention)
        {
            mention = null;

            if (!HasMention(text))
                return text;

            int startPos = text.IndexOf('<');
            int endPos = text.IndexOf('>');

            if (startPos - 1 >= 0 && text[startPos - 1] == ' ')
                startPos--;

            if (endPos + 1 < text.Length && text[endPos + 1] == ' ')
                endPos++;

            mention = text.Substring(startPos, endPos - startPos + 1);

            return text.Replace(mention, string.Empty);
        }
    }
}
