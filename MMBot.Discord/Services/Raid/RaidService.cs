using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MMBot.Data.Services.Interfaces;
using MMBot.Discord.Services;
using MMBot.Discord.Services.Interfaces;
using MMBot.Discord.Services.Raid;

namespace MMBot.Discord.Discord.Services.Raid
{
    public class RaidService : MMBotService<RaidService>, IRaidService
    {
        private readonly IDatabaseService _databaseService;
        private readonly IGuildSettingsService _guildSettings;
        private readonly GoogleSheetsService _googleSheetsService;

        public RaidService(IDatabaseService databaseService, IGuildSettingsService guildSettings, ILogger<RaidService> logger, GoogleSheetsService googleSheetsService) : base(logger)
        {
            _databaseService = databaseService;
            _guildSettings = guildSettings;
            _googleSheetsService = googleSheetsService;
        }

        public async Task ConnectAsync() => await _googleSheetsService?.ConnectAsync();
        public async Task<byte[]> GetTacticPicture() => await _googleSheetsService?.GetTacticPictureAsync();
    }
}
