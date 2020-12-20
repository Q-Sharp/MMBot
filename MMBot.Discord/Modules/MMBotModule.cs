using Discord.Addons.Interactive;
using Discord.Commands;
using MMBot.Data.Services.Interfaces;
using MMBot.Discord.Services.Interfaces;

namespace MMBot.Discord.Modules
{
    public abstract class MMBotModule : InteractiveBase<SocketCommandContext>
    {
        protected IDatabaseService _databaseService;
        protected ICommandHandler _commandHandler;
        protected IGuildSettingsService _guildSettings;

        public MMBotModule(IDatabaseService databaseService, IGuildSettingsService guildSettings, ICommandHandler commandHandler)
        {
            _databaseService = databaseService;
            _commandHandler = commandHandler;
            _guildSettings = guildSettings;
        }

        public static MMBotResult FromSuccess(string successMessage = null) => MMBotResult.Create(null, successMessage);
        public static MMBotResult FromError(CommandError error, string reason) => MMBotResult.Create(error, reason);
        public static MMBotResult FromErrorObjectNotFound(string objectname, string searchstring) => MMBotResult.Create(CommandError.ObjectNotFound, $"{objectname}: {searchstring}");
        public static MMBotResult FromErrorUnsuccessful(string error) => MMBotResult.Create(CommandError.Unsuccessful, error);
    }

    public class MMBotResult : RuntimeResult
    {
        private MMBotResult(CommandError? error, string reason = null) : base(error, reason)
        {
        }

        public static MMBotResult Create(CommandError? error, string reason = null) => new MMBotResult(error, reason);
    }
}
