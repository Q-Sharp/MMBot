using System.ComponentModel.DataAnnotations;
using MMBot.Data.Interfaces;

namespace MMBot.Data.Entities
{
    public class Channel : IHaveId
    {
        [Key]
        public int Id { get; set; }

        public ulong GuildId { get; set; }
        
        public ulong TextChannelId { get; set; }

        public ulong AnswerTextChannelId { get; set; }

        public void Update(object channel)
        {
            if(channel is Channel c && Id == c.Id)
            {
                GuildId = c.GuildId;
                TextChannelId = c.TextChannelId;
                AnswerTextChannelId = c.AnswerTextChannelId;
            }
        }
    }
}
