namespace MMBot.Discord.Services.Interfaces;

public interface IGuildSettingsService
{
    Task<Data.Contracts.Entities.GuildSettings> GetGuildSettingsAsync(ulong guildId);
}
