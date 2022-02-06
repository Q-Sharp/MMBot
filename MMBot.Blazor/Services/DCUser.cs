namespace MMBot.Blazor.Services;

public class DCUser : IDCUser
{
    public string CurrentGuildId { get; set; }
    public string Name { get; set; }
    public ulong Id { get; set; }
    public IList<DCChannel> Guilds { get; set; } = new List<DCChannel>();
    public string AvatarUrl { get; set; }
}
