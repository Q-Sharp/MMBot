using System;
using System.Threading.Tasks;

namespace MMBot.Discord.Services.Interfaces
{
    public interface ICsvService
    {
        Task<Exception> ImportCsv(byte[] csv, ulong guildID);
        Task<byte[]> ExportCsv(ulong guildID);
    }
}
