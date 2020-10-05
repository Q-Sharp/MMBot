using System.ComponentModel.DataAnnotations;

namespace TTMMBot.Data.Entities
{
    public class Channel
    {
        [Key]
        public int ChannelId { get; set; }

        public ulong GuildId { get; set; }
        
        public ulong TextChannelId { get; set; }
    }
}
