using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MMBot.Data;

namespace MMBot.Services.Interfaces
{
    public interface ICsvService : IMMBotInterface, IGuildSetter
    {
        Task<Exception> ImportCsv(byte[] csv);
        Task<byte[]> ExportCsv();
    }
}
