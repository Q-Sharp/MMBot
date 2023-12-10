namespace MMBot.Discord.Helpers;

public static class StringHelpers
{
    public static string ToSentence(this string input)
        => new(input.SelectMany((c, i) => i > 0 && char.IsUpper(c) ? [' ', c] : [c]).ToArray());

    public static string[] GetUrl(this string text)
        => text.Split("\t\n ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Where(s => s.StartsWith("http://") || s.StartsWith("www.") || s.StartsWith("https://")).ToArray();
}
