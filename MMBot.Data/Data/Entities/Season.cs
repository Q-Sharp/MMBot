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
    public class Season : IHaveId, IHaveIdentifier, IHaveGuildId
    {
        public int Id { get; set; }
        public int No { get; set; }
        public EraType Era { get; set; }
        public DateTime Start { get; set; }

        public string Identitfier => $"{Start} {Era} {No}";

        public string Name => Identitfier;

        public ulong GuildId { get; set; }

       

        [Display]
        public int? SHigh { get; set; }

        [Display]
        public int? Donations { get; set; }

        [JsonIgnore]
        public ICollection<Member> Member { get; set; }

        public void Update(object season)
        {
            if (season is Season m && (Id == m.Id || Name == m.Name))
            {
                
                SHigh = m.SHigh;
                Donations = m.Donations;
            }
        }
    }
}
