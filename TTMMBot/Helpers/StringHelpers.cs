using System.Linq;

namespace TTMMBot.Helpers
{
    public static class StringHelpers
    {
        public static string ToSentence(this string input) => new string(input.SelectMany((c, i) => i > 0 && char.IsUpper(c) ? new[] { ' ', c } : new[] { c }).ToArray());
    }
}
