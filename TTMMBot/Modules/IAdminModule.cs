using System.Threading.Tasks;
using Discord.Commands;
using TTMMBot.Services;

namespace TTMMBot.Modules
{
    public interface IAdminModule
    {
        IDatabaseService DatabaseService { get; set; }
        INotionCsvService CsvService { get; set; }
        IAdminService AdminService { get; set; }
        IGlobalSettings GlobalSettings { get; set; }
        Task ImportCsv();
        Task ExportCsv();
        Task ReorderJoin();
        Task Restart();
        Task FixRoles();
        Task DeleteDb();
    }
}