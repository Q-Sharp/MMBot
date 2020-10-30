using System;
using System.ComponentModel.DataAnnotations;
using MMBot.Helpers;

namespace MMBot.Data.Entities
{
    public class MMTimer : IHaveId
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display]
        public string Name { get; set; }

        [Display]
        public bool IsRecurring { get; set; }

        [Display]
        public bool IsActive { get; set; }

        public DateTime? StartTime { get; set; }

        [Display]
        public TimeSpan? RingSpan { get; set; }

        [Display]
        public DateTime? EndTime { get; set; }

        public ulong GuildId { get; set; }

        public ulong? ChannelId { get; set; }

        [Display]
        public string Message { get; set; }

        public void Update(object timer)
        {
           if(timer is MMTimer t && Id == t.Id)
                this.ChangeProperties(t);
        }

        public override string ToString() => $"{Name}";
    }
}
