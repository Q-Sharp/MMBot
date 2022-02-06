using MMBot.Data;

namespace MMBot.Discord.Services.Interfaces;

public interface IJsonService
{
    Task<IDictionary<string, string>> ExportDBToJson();
    Task<bool> ImportJsonToDB(IDictionary<string, string> importJson, Context context = null);
}
