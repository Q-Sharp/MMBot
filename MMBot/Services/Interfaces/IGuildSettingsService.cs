using System;
using System.Threading.Tasks;
using MMBot.Data.Entities;

namespace MMBot.Services.Interfaces
{
    public interface IGuildSettingsService
    {
        Task<GuildSettings> GetGuildSettingsAsync(ulong guildId);
    }
}
