﻿namespace MMBot.Data.Contracts.Entities;

public class RaidParticipation : IHaveId, IHaveGuildId
{
    public int Id { get; set; }

    public int RaidParticipationId { get; set; }
    public int MemberId { get; set; }
    public int RaidBossId { get; set; }

    public virtual RaidBoss RaidBoss { get; set; }
    public virtual Member Member { get; set; }

    public ulong DamageDone { get; set; }
    public int HeartQty { get; set; }
    public int AttackQty { get; set; }
    public ulong GuildId { get; set; }

    public object Update(object raidBoss) => throw new NotImplementedException();

    public byte[] Version { get; set; }
}
