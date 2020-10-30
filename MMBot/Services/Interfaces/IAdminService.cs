using System.Threading.Tasks;
using MMBot.Data;

namespace MMBot.Services.Interfaces
{
    public interface IAdminService : IMMBotInterface, IGuildSetter
    {
        Task Reorder();
        Task DeleteDb();
        Task Restart();
    }
}
