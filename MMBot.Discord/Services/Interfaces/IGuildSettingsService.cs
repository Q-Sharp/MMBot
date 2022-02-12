namespace MMBot.Discord.Services.Interfaces;

public interface IGuildSettingsService
{
    Task<Data.Entities.GuildSettings> GetGuildSettingsAsync(ulong guildId);
}
