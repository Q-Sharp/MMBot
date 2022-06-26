namespace MMBot.Data.Contracts;

public interface IBlazorDatabaseService
{
    IList<Tuple<ulong, string>> GetAllGuilds();
}