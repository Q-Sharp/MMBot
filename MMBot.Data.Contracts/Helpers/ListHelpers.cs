namespace MMBot.Data.Contracts.Helpers;

public static class ListHelpers
{
    public static IList<IList<T>> ChunkBy<T>(this IList<T> source, int chunkSize)
        => source?.Select((x, i) => new { Index = i, Value = x })
                 .GroupBy(x => x.Index / chunkSize)
                 .Select(x => x.Select(v => v.Value).ToList() as IList<T>)
                 .ToList();

    public static void ForEach<T>(this IEnumerable<T> n, Action<T> a)
        => n?.ToList().ForEach(a);

    public static T[] Shuffle<T>(this IList<T> sequence)
    {
        if (sequence?.Count is null or 0)
            return sequence?.ToArray();

        var random = new Random((int)DateTime.UtcNow.Ticks);
        var retArray = sequence.ToArray();

        var n = retArray.Length;
        while (n > 1)
        {
            var k = random.Next(n--);
            (retArray[k], retArray[n]) = (retArray[n], retArray[k]);
        }

        return retArray.ToArray();
    }
}
