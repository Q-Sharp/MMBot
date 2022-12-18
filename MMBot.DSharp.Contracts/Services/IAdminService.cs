namespace MMBot.DSharp.Contracts.Services;

public interface IAdminService
{
    Task Reorder(ulong guildId);
    void Truncate();
    Task Restart(bool saveRestart = false, ulong? guildId = null, ulong? channelId = null);
    Task<bool> DataImport(byte[] zipBytes);
}
