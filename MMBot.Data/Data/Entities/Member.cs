using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using MMBot.Data.Enums;
using MMBot.Data.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MMBot.Data.Entities
{
    public class Member : IHaveId, IHaveIdentifier, IHaveGuildId
    {
        public int Id { get; set; }

        [Required]
        [Display]
        public virtual string Name { get; set; }

        [JsonIgnore]
        public virtual string Identitfier => Name;

        [Display]
        [IgnoreDataMember]
        [JsonIgnore]
        public virtual string ClanTag => Clan?.Tag;

        [Display]
        public virtual string Discord { get; set; }

        [Display]
        public virtual int? AHigh { get; set; }
        public virtual int? Current { get; set; }

        [Display]
        public virtual Role Role { get; set; }

        public virtual DiscordStatus DiscordStatus { get; set; } = DiscordStatus.Active;

        [Display]
        public virtual bool IsActive { get; set; }

        public virtual int? ClanId { get; set; }

        [JsonIgnore]
        public virtual Clan Clan { get; set; }

        [JsonIgnore]
        public virtual ICollection<Vacation> Vacation { get; set; } = new Collection<Vacation>();

        public virtual DateTime? LastUpdated { get; set; }

        public virtual int Join { get; set; }

        [Display]
        public virtual bool IgnoreOnMoveUp { get; set; }

        [Display]
        public virtual string PlayerTag { get; set; }

        public virtual bool AutoSignUpForFightNight { get; set; }

        public virtual ulong GuildId { get; set; }

        public virtual int? MemberGroupId { get; set; }

        [JsonIgnore]
        public virtual MemberGroup MemberGroup { get; set; }

        [JsonIgnore]
        [Display]
        public virtual int? SHighLowest => MemberGroup?.Members.Min(x => x?.Season?.LastOrDefault()?.SHigh);

        [JsonIgnore]
        [Display]
        public virtual int? SHigh => Season?.LastOrDefault()?.SHigh;

        [Display]
        public virtual double? LocalTimeOffSet { get; set; }

        [Display]
        [IgnoreDataMember]
        [JsonIgnore]
        public DateTime? LocalDateTime 
            => LocalTimeOffSet.HasValue ? DateTime.UtcNow + TimeSpan.FromHours(LocalTimeOffSet.Value) : null;

        [JsonIgnore]
        public virtual ICollection<Strike> Strikes { get; set; } = new Collection<Strike>();

        [JsonIgnore]
        public virtual ICollection<RaidParticipation> RaidParticipation { get; set; } = new Collection<RaidParticipation>();

        [JsonIgnore]
        public virtual ICollection<Season> Season { get; set; } = new Collection<Season>();

        public override string ToString() => Clan?.Tag is not null ? $"[{Clan?.Tag}] {Name}" : $"{Name}";

        public void Update(object member)
        {
            if(member is Member m && (Id == m.Id || Name == m.Name))
            {
                Name = m.Name;
                Discord = m.Discord;
                AHigh = m.AHigh;
                Role = m.Role;
                DiscordStatus = m.DiscordStatus;
                IsActive = m.IsActive;
                ClanId = m.ClanId;
                LastUpdated = m.LastUpdated;
                Join = m.Join;
                IgnoreOnMoveUp = m.IgnoreOnMoveUp;
                PlayerTag = m.PlayerTag;
                AutoSignUpForFightNight = m.AutoSignUpForFightNight;
                GuildId = m.GuildId;
                MemberGroupId = m.MemberGroupId;
                LocalTimeOffSet = m.LocalTimeOffSet;
            }
        }
    }
}
