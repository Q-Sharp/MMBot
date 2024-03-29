﻿namespace MMBot.Data.Contracts.Entities;

public class GuildSettings : IHaveId, IHaveGuildId
{
    public int Id { get; set; }

    [Display]
    public ulong GuildId { get; set; }

    [Display]
    public string GuildName { get; set; }

    [Display]
    public string Prefix { get; set; }

    // Filesystem
    [Display]
    public string FileName { get; set; }

    // InGame
    [Display]
    public int ClanSize { get; set; }

    [Display]
    public int MemberMovementQty { get; set; }

    [Display]
    public ulong CategoryId { get; set; }

    [Display]
    public ulong MemberRoleId { get; set; }

    public byte[] Version { get; set; }

    public object Update(object guildSettings)
    {
        if (guildSettings is GuildSettings gs && Id == gs.Id)
        {
            GuildId = gs.GuildId;
            Prefix = gs.Prefix;
            FileName = gs.FileName;
            ClanSize = gs.ClanSize;
            MemberMovementQty = gs.MemberMovementQty;
            CategoryId = gs.CategoryId;
            MemberRoleId = gs.MemberRoleId;
        }
        return this;
    }
}
