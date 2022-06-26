using MMBot.Blazor.Shared.Helpers;

namespace MMBot.Blazor.Shared.Auth;

public class DCChannel
{
    public string Id { get; set; }
    public string name { get; set; }
    public bool owner { get; set; }
    public int permissions { get; set; }

    public GuildPermission PermissionFlags => (GuildPermission)permissions;
}
