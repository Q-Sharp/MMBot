using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
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

                if (e.Original.ClanID != e.Entity.ClanID)
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
        public int MemberID { get; set; }

        public string Discord { get; set; }

        [Required]
        public string Name { get; set; }

        public int? AllTimeHigh { get; set; }

        public int? SeasonHighest { get; set; }

        public int? Donations { get; set; }

        [NotMapped]
        public int? TotalScore => (5 * SeasonHighest + 2 * AllTimeHigh + 1 * Donations) / 3;

        public Role Role { get; set; }
        public bool IsActive { get; set; }

        public int? ClanID { get; set; }
        public virtual Clan Clan { get; set; }

        public virtual ICollection<Vacation> Vacation { get; set; }

        public DateTime? LastUpdated { get; set; }

        public int JoinOrder { get; set; }

        public override string ToString() => Clan?.Tag != null ? $"[{Clan?.Tag}] {Name}" : $"{Name}";

        private static void ReorderJoin(IEntry<Member, Context> e, Member ce = null)
        {
            e.Context.Member.AsQueryable()
                .Where(x => x.ClanID == e.Entity.ClanID)
                .OrderBy(x => x.JoinOrder)
                .Select((m, i) => new { i, m })
                .ToList()
                .ForEach(mi => mi.m.JoinOrder = mi.i);
            e.Context.SaveChanges();

            if (ce != null)
            {
                ce.JoinOrder = ce.Clan.Member.AsQueryable()
                    .Where(m => m.JoinOrder > 0)
                    .Max(x => x.JoinOrder) + 1;
                e.Context.SaveChanges();
            }
        }
    }
}
