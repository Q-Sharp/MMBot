﻿namespace MMBot.Data.Contracts.Entities;

public class Clan : IHaveId, IHaveIdentifier, IHaveGuildId
{
    public virtual int Id { get; set; }

    public virtual int SortOrder { get; set; }

    [Required]
    [Display]
    public virtual string Tag { get; set; }

    [JsonIgnore]
    public virtual string Identitfier => Tag;

    [Display]
    public virtual string Name { get; set; }
    public virtual string DiscordRole { get; set; }

    public virtual ulong GuildId { get; set; }

    public virtual ICollection<Member> Member { get; set; } = new Collection<Member>();

    public virtual ICollection<RaidBoss> RaidBoss { get; set; } = new Collection<RaidBoss>();

    public byte[] Version { get; set; }

    public override string ToString() => $"[{Tag}]";

    public object Update(object clan)
    {
        if (clan is Clan c && (Id == c.Id || Name == c.Name))
        {
            SortOrder = c.SortOrder;
            Tag = c.Tag;
            Name = c.Name;
            DiscordRole = c.DiscordRole;
            GuildId = c.GuildId;
        }
        return this;
    }
}
