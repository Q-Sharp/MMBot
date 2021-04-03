//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using MMBot.Data.Enums;
//using MMBot.Data.Interfaces;

//namespace MMBot.Data.Entities
//{
//    public class RaidBossModel : RaidBoss
//    {
//        public event Action StateChanged;
//        private void NotifyStateChanged() => StateChanged?.Invoke();

//        public int Id { get; set; }
//        public ulong GuildId { get; set; }
//        public BossType BossType { get; set; }
//        public string Name => $"{BossType}";
//        public string Identitfier => Name;

//        public ModifierOne ModifierOne { get; set; }
//        public ModifierTwo ModifierTwo { get; set; }
//        public ModifierThree ModifierThree { get; set; }

//        public virtual ICollection<RaidParticipationModel> RaidParticipation { get; set; } = new Collection<RaidParticipationModel>();
//        public void Update(object guildSettings) => throw new NotImplementedException();
//    }
//}
