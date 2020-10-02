using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TTMMBot.Data;

namespace TTMMBot.Services.Interfaces
{
    public interface INotionCsvService
    {
        Context Context { get; set; }
        GlobalSettingsService Settings { get; set; }
        ILogger<NotionCsvService> Logger { get; set; }
        Task<Exception> ImportCsv(byte[] csv);
        Task<byte[]> ExportCsv();
    }
}