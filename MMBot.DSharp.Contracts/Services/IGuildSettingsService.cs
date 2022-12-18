namespace MMBot.DSharp.Contracts.Services;

public interface IGuildSettingsService
{
    Task<Data.Contracts.Entities.GuildSettings> GetGuildSettingsAsync(ulong guildId);
}
