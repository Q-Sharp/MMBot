using System.ComponentModel.DataAnnotations;
using MMBot.Data.Interfaces;

namespace MMBot.Data.Entities
{
    public class Restart : IHaveId
    {
        public int Id { get; set; }

        public ulong Guild { get; set; }
        public ulong Channel { get; set; }

        public void Update(object restart)
        {
            if(restart is Restart r && Id == r.Id)
            {
                Guild = r.Guild;
                Channel = r.Channel;
            }
        }
    }
}
