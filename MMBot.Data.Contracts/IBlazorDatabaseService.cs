namespace MMBot.Data.Contracts;

public interface IBlazorDatabaseService
{
    IEnumerable<Guild> GetAllGuilds();
}