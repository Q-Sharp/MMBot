using System.Threading.Tasks;
using TTMMBot.Data;

namespace TTMMBot.Services.Interfaces
{
    public interface IAdminService : IMMBotInterface, IGuildSetter
    {
        Task Reorder();
    }
}
