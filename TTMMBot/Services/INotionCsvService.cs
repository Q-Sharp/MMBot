using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TTMMBot.Data;

namespace TTMMBot.Services
{
    public interface INotionCsvService
    {
        Context Context { get; set; }
        GlobalSettings Settings { get; set; }
        ILogger<NotionCsvService> Logger { get; set; }
        Task<Exception> ImportCsv(byte[] csv);
        Task<byte[]> ExportCsv();
    }
}