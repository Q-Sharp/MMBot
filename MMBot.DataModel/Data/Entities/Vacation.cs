using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MMBot.Data.Interfaces;

namespace MMBot.Data.Entities
{
    public class Vacation : IHaveId
    {
        [Key]
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int MemberId { get; set; }

        [JsonIgnore]
        public virtual Member Member { get; set; }

        public void Update(object vacation)
        {
            if(vacation is Vacation v && Id == v.Id)
            {
                StartDate = v.StartDate;
                EndDate = v.EndDate;
                MemberId = v.MemberId;
            }
        }
    }
}
