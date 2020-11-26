using System.Threading.Tasks;

namespace MMBot.Services.Interfaces
{
    public interface IGoogleSheetsService
    {
        Task ConnectAsync();

        Task<byte[]> GetTacticPictureAsync();
    }
}