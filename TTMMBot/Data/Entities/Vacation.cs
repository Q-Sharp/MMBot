using System;
using System.ComponentModel.DataAnnotations;

namespace TTMMBot.Data.Entities
{
    public class Vacation
    {
        [Key]
        public int VacationId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int MemberId { get; set; }
        public virtual Member Member { get; set; }
    }
}
