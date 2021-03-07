using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MMBot.Data.Enums;
using MMBot.Data.Interfaces;

namespace MMBot.Data.Entities
{
    public class RaidBoss : IHaveId, IHaveIdentifier
    {
        public int Id { get; set; }
        public ulong GuildId { get; set; }
        public BossType BossType { get; set; }
        public string Name => $"{BossType}";
        public string Identitfier => Name;

        public ModifierOne ModifierOne { get; set; }
        public ModifierTwo ModifierTwo { get; set; }
        public ModifierThree ModifierThree { get; set; }

        public virtual ICollection<RaidParticipation> RaidParticipation { get; set; } = new Collection<RaidParticipation>();
        public void Update(object guildSettings) => throw new NotImplementedException();
    }
}
