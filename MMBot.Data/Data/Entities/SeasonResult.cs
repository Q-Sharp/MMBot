using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Discord;
using MMBot.Data.Enums;
using MMBot.Data.Interfaces;

namespace MMBot.Data.Entities
{
    public class SeasonResult : IHaveId, IHaveGuildId
    {
        public int Id { get; set; }
        public int No { get; set; }
        public ulong GuildId { get; set; }
        public int MemberId { get; set; }
        public int SeasonId { get; set; }

        [Display]
        public int? SHigh { get; set; }

        [Display]
        public int? Donations { get; set; }

        public Member Member { get; set; }
        public Season Season { get; set; }

        public void Update(object season)
        {
            if (season is SeasonResult m && (Id == m.Id))
            {

            }
        }
    }
}
