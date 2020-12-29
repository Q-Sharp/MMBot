using System.Threading.Tasks;
using MMBot.Data;

namespace MMBot.Discord.Services.Interfaces
{
    public interface IAdminService
    {
        Task Reorder(ulong guildId);
        Task<Context> DeleteDb();
        void Restart();
    }
}
