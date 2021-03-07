using System;
using MMBot.Data.Interfaces;

namespace MMBot.Data.Entities
{
    public class RaidParticipation : IHaveId
    {
        public int Id { get; set; }

        public int RaidParticipationId { get; set; }
        public int MemberId { get; set; }
        
        public RaidBoss BossRaid { get; set; }
        public Member Member { get; set; }

        public ulong DamageDone { get; set; }
        public int HeartQty { get; set; }
        public int AttackQty { get; set; }

        public void Update(object bossRaider) => throw new NotImplementedException();
    }
}
