using System.ComponentModel.DataAnnotations;
using MMBot.Helpers;

namespace MMBot.Data.Entities
{
    public class Restart : IHaveId
    {
        [Key]
        public int Id { get; set; }

        public ulong Guild { get; set; }
        public ulong Channel { get; set; }

        public void Update(object restart)
        {
            if(restart is Restart r && Id == r.Id)
                this.ChangeProperties(r);
        }
    }
}
