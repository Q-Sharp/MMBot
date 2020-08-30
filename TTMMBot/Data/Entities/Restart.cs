using System.ComponentModel.DataAnnotations;

namespace TTMMBot.Data.Entities
{
    public class Restart
    {
        [Key]
        public int RestartId { get; set; }

        public ulong Guild { get; set; }
        public ulong Channel { get; set; }
    }
}
