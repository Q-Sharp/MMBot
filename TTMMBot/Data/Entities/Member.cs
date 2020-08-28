using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using EntityFrameworkCore.Triggers;
using TTMMBot.Data.Enums;

namespace TTMMBot.Data.Entities
{
    public class Member
    {
        static Member()
        {
            Triggers<Member, Context>.Updating += e =>
            {
                if (!e.Context.UseTriggers)
                    return;

                if (e.Original.ClanId != e.Entity.ClanId)
                    ReorderJoin(e);
            };

            Triggers<Member, Context>.Deleted += e =>
            {
                if (!e.Context.UseTriggers)
                    return;

                ReorderJoin(e);
            };
        }

        [Key]
        public int MemberId { get; set; }

        [Required]
        [Display]
        public string Name { get; set; }

        [Display]
        [IgnoreDataMember]
        public string ClanTag => Clan?.Tag;

        [Display]
        public string Discord { get; set; }

        [Display]
        public int? AHigh { get; set; }

        [Display]
        public int? SHigh { get; set; }

        [Display]
        public int? Donations { get; set; }

        [Display]
        public Role Role { get; set; }

        public DiscordStatus DiscordStatus { get; set; } = DiscordStatus.Active;

        public bool IsActive { get; set; }

        public int? ClanId { get; set; }
        public virtual Clan Clan { get; set; }

        public virtual ICollection<Vacation> Vacation { get; set; }

        public DateTime? LastUpdated { get; set; }

        public int Join { get; set; }

        public bool IgnoreOnMoveUp { get; set; }

        public override string ToString() => Clan?.Tag != null ? $"[{Clan?.Tag}] {Name}" : $"{Name}";

        private static void ReorderJoin(IEntry<Member, Context> e, Member ce = null)
        {
            e.Context.Member.AsQueryable()
                .Where(x => x.ClanId == e.Entity.ClanId)
                .OrderBy(x => x.Join)
                .Select((m, i) => new { i, m })
                .ToList()
                .ForEach(mi => mi.m.Join = mi.i);
            e.Context.SaveChanges();

            if (ce == null) 
                return;

            ce.Join = ce.Clan.Member.AsQueryable()
                    .Where(m => m.Join > 0)
                    .Max(x => x.Join) + 1;

                e.Context.SaveChanges();
        }
    }
}
