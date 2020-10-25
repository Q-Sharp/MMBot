using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using TTMMBot.Data.Enums;

namespace TTMMBot.Data.Entities
{
    public class MemberGroup
    {
        [Key]
        public int MemberGroupId { get; set; }

        public virtual ICollection<Member> Members { get; set; } 
    }
}
