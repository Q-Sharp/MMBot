namespace MMBot.Data.Contracts.Entities;

public class RaidParticipation : IHaveId, IHaveGuildId
{
    public int Id { get; set; }

    public int RaidParticipationId { get; set; }
    public int MemberId { get; set; }
    public int RaidBossId { get; set; }

    public RaidBoss RaidBoss { get; set; }
    public Member Member { get; set; }

    public ulong DamageDone { get; set; }
    public int HeartQty { get; set; }
    public int AttackQty { get; set; }
    public ulong GuildId { get; set; }

    public void Update(object raidBoss) => throw new NotImplementedException();
}
