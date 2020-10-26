using System.ComponentModel.DataAnnotations;
using TTMMBot.Helpers;

namespace TTMMBot.Data.Entities
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
                this.ChangeProperties(c);
        }
    }
}
