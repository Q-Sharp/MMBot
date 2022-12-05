namespace MMBot.Blazor.Shared.Auth;

public class DCChannel
{
    public string Id { get; set; }
    public string Name { get; set; }
    public bool Owner { get; set; }
    public int Permissions { get; set; }

    public bool IsSelected { get; set; } = false;

    public GuildPermission PermissionFlags => (GuildPermission)Permissions;
}
