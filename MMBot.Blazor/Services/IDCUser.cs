using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMBot.Blazor.Services
{
    public interface IDCUser
    {
        string CurrentGuildId { get; }
        Task<string> GetCurrentGuildId();
        ulong? CurrentGuildIdUlong { get; }
        Task SetCurrentGuildId(string guildId);
        IList<DCChannel> Guilds { get; set; }
        string Name { get; set; }
    }
}