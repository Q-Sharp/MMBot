using System;
using System.Collections.Generic;
using System.Linq;

namespace MMBot.Data.Helpers
{
    public static class ListHelpers
    {
        public static IList<IList<T>> ChunkBy<T>(this IList<T> source, int chunkSize)
            => source.Select((x, i) => new { Index = i, Value = x })
                     .GroupBy(x => x.Index / chunkSize)
                     .Select(x => x.Select(v => v.Value).ToList() as IList<T>)
                     .ToList();

        public static void ForEach<T>(this IEnumerable<T> n, Action<T> a) => n.ToList().ForEach(a);

        public static T[] Shuffle<T>(this IList<T> sequence)
        {
            var random = new Random((int)DateTime.UtcNow.Ticks);
            var retArray = sequence.ToArray();

            var n = retArray.Length;       
            while (n > 1)
            {
               var k = random.Next(n--);   
               var temp = retArray[n];     
               retArray[n] = retArray[k];
               retArray[k] = temp;
            }

            return retArray.ToArray();
        }
    }
}
