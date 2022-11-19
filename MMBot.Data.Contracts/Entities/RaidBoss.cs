using System.Collections.ObjectModel;
using MMBot.Data.Contracts.Enums;

namespace MMBot.Data.Contracts.Entities;

public class RaidBoss : IHaveId, IHaveIdentifier, IHaveGuildId
{
    public int Id { get; set; }
    public ulong GuildId { get; set; }
    public BossType BossType { get; set; }
    public string Name => $"{BossType}";
    public string Identitfier => Name;

    public ModifierOne ModifierOne { get; set; }
    public ModifierTwo ModifierTwo { get; set; }
    public ModifierThree ModifierThree { get; set; }

    public int ClanId { get; set; }
    public virtual Clan Clan { get; set; }
    public virtual ICollection<RaidParticipation> RaidParticipation { get; set; } = new Collection<RaidParticipation>();
    public void Update(object guildSettings) => throw new NotImplementedException();

    public byte[] Version { get; set; }
}
