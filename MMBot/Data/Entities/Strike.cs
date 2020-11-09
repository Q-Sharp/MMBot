﻿using System;
using System.ComponentModel.DataAnnotations;

namespace MMBot.Data.Entities
{
    public class Strike : IHaveId
    {
        [Key]
        public int Id { get; set; }

        public string Reason { get; set; }

        public DateTime? StrikeDate { get; set; }

        public int MemberId { get; set; }

        public virtual Member Member { get; set; }

         public void Update(object strike)
        {
            if(strike is Strike m && Id == m.Id)
            {
                Reason = m.Reason;
                StrikeDate = m.StrikeDate;
                MemberId = m.MemberId;
                Member = m.Member;
            }
        }
    }
}