﻿using System.Threading.Tasks;
using Discord.Commands;

namespace MMBot.Modules.Interfaces
{
    public interface IAdminModule
    {
        Task<RuntimeResult> ImportCsv();
        Task<RuntimeResult> ExportCsv();
        Task<RuntimeResult> ReorderJoin();
        Task<RuntimeResult> Restart(bool saveRestart = true);
        Task<RuntimeResult> FixRoles();
        Task<RuntimeResult> DeleteDb();
    }
}
