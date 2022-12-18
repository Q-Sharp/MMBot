namespace MMBot.DSharp.Contracts.Services;

public interface IJsonService
{
    Task<IDictionary<string, string>> ExportDBToJson();
    Task<bool> ImportJsonToDB(IDictionary<string, string> importJson, Context context = null);
}
