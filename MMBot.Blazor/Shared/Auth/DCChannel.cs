namespace MMBot.Blazor.Shared.Auth;

public class DCChannel
{
    public string Id { get; set; }
    public string Name { get; set; }
    public bool Owner { get; set; }
    public int Permissions { get; set; }

    public bool IsSelected { get; set; } = false;

    public GuildPermission PermissionFlags => (GuildPermission)Permissions;

    public override bool Equals(object o)
        => (o as DCChannel)?.Id == Id;

    public override int GetHashCode() => Id?.GetHashCode() ?? 0;
    public override string ToString() => Name;
}
