﻿using System.Threading.Tasks;
using MMBot.Data;

namespace MMBot.Services.Interfaces
{
    public interface IAdminService
    {
        Task Reorder(ulong guildId);
        Task DeleteDb();
        Task Restart();
    }
}
