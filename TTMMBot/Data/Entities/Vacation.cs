using System;
using System.ComponentModel.DataAnnotations;
using TTMMBot.Helpers;

namespace TTMMBot.Data.Entities
{
    public class Vacation : IHaveId
    {
        [Key]
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int MemberId { get; set; }
        public virtual Member Member { get; set; }

        public void Update(object vacation)
        {
            if(vacation is Vacation v && Id == v.Id)
                this.ChangeProperties(v);
        }
    }
}
