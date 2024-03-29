﻿namespace MMBot.Blazor.Shared.Auth;

public class DCUser
{
    public static DCUser Anonymous => new();

    public string Name { get; set; }
    public ulong Id { get; set; }
    public ICollection<DCChannel> Guilds { get; set; } = new List<DCChannel>();
    public string AvatarUrl { get; set; }

    public bool IsAuthenticated { get; set; }

    public string NameClaimType { get; set; }

    public string RoleClaimType { get; set; }

    public ICollection<ClaimValue> Claims { get; set; } = new List<ClaimValue>();
}
