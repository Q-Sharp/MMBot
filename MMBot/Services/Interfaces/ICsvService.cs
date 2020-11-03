using System;
using System.Threading.Tasks;

namespace MMBot.Services.Interfaces
{
    public interface ICsvService
    {
        Task<Exception> ImportCsv(byte[] csv, ulong guildID);
        Task<byte[]> ExportCsv(ulong guildID);
    }
}
