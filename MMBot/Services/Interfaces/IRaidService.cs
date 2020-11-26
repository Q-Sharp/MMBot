using System.Threading.Tasks;
using MMBot.Data;

namespace MMBot.Services.Interfaces
{
    public interface IRaidService
    {
        Task ConnectAsync();
        Task<byte[]> GetTacticPicture();
    }
}
