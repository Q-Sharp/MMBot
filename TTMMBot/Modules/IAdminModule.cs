using System.Threading.Tasks;
using TTMMBot.Services;
using TTMMBot.Services.Interfaces;

namespace TTMMBot.Modules
{
    public interface IAdminModule
    {
        IDatabaseService DatabaseService { get; set; }
        INotionCsvService CsvService { get; set; }
        IAdminService AdminService { get; set; }
        GlobalSettingsService GlobalSettings { get; set; }
        Task ImportCsv();
        Task ExportCsv();
        Task ReorderJoin();
        Task Restart(bool saveRestart = true);
        Task FixRoles();
        Task DeleteDb();
    }
}