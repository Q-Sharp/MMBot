namespace MMBot.Blazor.Shared;

public interface ISelectedGuildService
{
    Task<IEnumerable<DCChannel>> GetGuilds();
    Task<ulong> GetSelectedGuildId();
    Task SetSelectedGuild(string id);
    event EventHandler<EventArgs> Changed;
}