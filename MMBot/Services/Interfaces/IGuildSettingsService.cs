using System;
using System.Threading.Tasks;

namespace MMBot.Services.Interfaces
{
    public interface IGuildSettingsService : IMMBotInterface, IGuildSetter
    {
        string Prefix { get; }
        TimeSpan WaitForReaction { get; }
        string FileName { get; }
        int ClanSize { get; }
        int MemberMovementQty { get; }
        ulong GuildId { get; }
    }
}
