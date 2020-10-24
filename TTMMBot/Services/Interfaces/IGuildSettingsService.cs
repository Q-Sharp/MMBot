using System;
using System.Threading.Tasks;

namespace TTMMBot.Services.Interfaces
{
    public interface IGuildSettingsService
    {
        string Prefix { get; }
        TimeSpan WaitForReaction { get; }
        string FileName { get; }
        int ClanSize { get; }
        int MemberMovementQty { get; }
        ulong GuildId { get; }

        void LoadSettings(ulong id);
    }
}
