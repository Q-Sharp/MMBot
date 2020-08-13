﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TTMMBot.Data.Entities
{
    public class Vacation
    {
        [Key]
        public int VacationID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int MemberID { get; set; }
        public virtual Member Member { get; set; }
    }
}
