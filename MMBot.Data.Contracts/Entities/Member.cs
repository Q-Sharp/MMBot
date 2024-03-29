﻿namespace MMBot.Data.Contracts.Entities;

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

    public virtual ulong GuildId { get; set; }

    public virtual int? MemberGroupId { get; set; }

    [JsonIgnore]
    public virtual MemberGroup MemberGroup { get; set; }

    [JsonIgnore]
    [Display]
    public virtual int? SHighLowest => MemberGroup?.Members.Min(x => x?.SeasonResult?.LastOrDefault()?.SHigh ?? 0);

    [JsonIgnore]
    [Display]
    public virtual int? SHigh => SeasonResult?.LastOrDefault()?.SHigh ?? 0;

    [Display]
    public virtual double? LocalTimeOffSet { get; set; }

    [Display]
    [IgnoreDataMember]
    [JsonIgnore]
    public DateTime? LocalDateTime
        => LocalTimeOffSet.HasValue ? DateTime.UtcNow + TimeSpan.FromHours(LocalTimeOffSet.Value) : null;

    [JsonIgnore]
    public virtual ICollection<Strike> Strike { get; set; } = new Collection<Strike>();

    [JsonIgnore]
    public virtual ICollection<RaidParticipation> RaidParticipation { get; set; } = new Collection<RaidParticipation>();

    [JsonIgnore]
    public virtual ICollection<SeasonResult> SeasonResult { get; set; } = new Collection<SeasonResult>();

    public override string ToString() => Clan?.Tag is not null ? $"[{Clan?.Tag}] {Name}" : $"{Name}";
    public byte[] Version { get; set; }

    public object Update(object member)
    {
        if (member is Member m && (Id == m.Id || Name == m.Name))
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
            GuildId = m.GuildId;
            MemberGroupId = m.MemberGroupId;
            LocalTimeOffSet = m.LocalTimeOffSet;
        }
        return this;
    }
}
