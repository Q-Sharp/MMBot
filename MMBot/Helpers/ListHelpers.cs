using System;
using System.Collections.Generic;
using System.Linq;

namespace MMBot.Helpers
{
    public static class ListHelpers
    {
        public static IList<IList<T>> ChunkBy<T>(this IList<T> source, int chunkSize)
            => source.Select((x, i) => new { Index = i, Value = x })
                     .GroupBy(x => x.Index / chunkSize)
                     .Select(x => x.Select(v => v.Value).ToList() as IList<T>)
                     .ToList();

        public static void ForEach<T>(this IEnumerable<T> n, Action<T> a) => n.ToList().ForEach(a);
    }
}
