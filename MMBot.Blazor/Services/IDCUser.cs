namespace MMBot.Blazor.Services;

public interface IDCUser
{
    IList<DCChannel> Guilds { get; set; }
    string Name { get; set; }
    ulong Id { get; set; }
    string AvatarUrl { get; set; }
}
