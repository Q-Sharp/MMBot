using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using MMBot.Helpers;

namespace MMBot.Data.Entities
{
    public class MemberGroup : IHaveId
    {
        [Key]
        public int Id { get; set; }

        [JsonIgnore]
        public virtual ICollection<Member> Members { get; set; }

        public void Update(object memberGroup)
        {
        }
    }
}
