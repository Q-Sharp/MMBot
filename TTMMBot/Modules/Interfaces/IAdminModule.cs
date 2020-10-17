using System.Threading.Tasks;

namespace TTMMBot.Modules.Interfaces
{
    public interface IAdminModule
    {
        Task ImportCsv();
        Task ExportCsv();
        Task ReorderJoin();
        Task Restart(bool saveRestart = true);
        Task FixRoles();
        Task DeleteDb();
    }
}
