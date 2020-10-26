using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TTMMBot.Helpers;

namespace TTMMBot.Data.Entities
{
    public class MemberGroup : IHaveId
    {
        [Key]
        public int Id { get; set; }

        public virtual ICollection<Member> Members { get; set; }

        public void Update(object memberGroup)
        {
            if (memberGroup is MemberGroup mg && Id == mg.Id)
                this.ChangeProperties(mg);
        }
    }
}
