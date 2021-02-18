using System.Collections.Generic;

namespace MMBot.Blazor.Services
{
    public class DCUser 
    {
        public string Name { get; set; }
        public string CurrentGuildId { get; set; }
        public ulong? CurrentGuildIdUlong 
        {
            get 
            {
                if(ulong.TryParse(CurrentGuildId, out var val))
                    return val;
                return null;
            }
        }
        public IList<DCChannel> Guilds { get; set; } = new List<DCChannel>();
    }
}
