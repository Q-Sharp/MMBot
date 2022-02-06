using Discord.Commands;

namespace MMBot.Discord.Modules.Interfaces;

public interface IAdminModule
{
    Task<RuntimeResult> ImportCsv();
    Task<RuntimeResult> ExportCsv();
    Task<RuntimeResult> ReorderJoin();
    Task<RuntimeResult> Restart(bool saveRestart = true);
    Task<RuntimeResult> FixRoles();
    Task<RuntimeResult> TruncateDb();
}
