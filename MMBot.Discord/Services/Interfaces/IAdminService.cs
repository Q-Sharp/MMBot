using System.Threading.Tasks;

namespace MMBot.Discord.Services.Interfaces
{
    public interface IAdminService
    {
        Task Reorder(ulong guildId);
        void Truncate();
        Task Restart(bool saveRestart = false, ulong? guildId = null, ulong? channelId = null, bool isDataImport = false);
        Task InitDataImport(ulong guildId, ulong channelId);
        Task<bool> FinishDataImport();
    }
}
