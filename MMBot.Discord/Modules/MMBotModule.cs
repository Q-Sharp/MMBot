using Discord;
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

        public static MMBotResult FromSuccess(string successMessage = null, IMessage answer = null) 
            => MMBotResult.Create(null, successMessage, answer);
        public static MMBotResult FromError(CommandError error, string reason, IMessage answer = null) 
            => MMBotResult.Create(error, reason, answer);
        public static MMBotResult FromErrorObjectNotFound(string objectname, string searchstring, IMessage answer = null) 
            => MMBotResult.Create(CommandError.ObjectNotFound, $"{objectname}: {searchstring}", answer);
        public static MMBotResult FromErrorUnsuccessful(string error, IMessage answer = null) 
            => MMBotResult.Create(CommandError.Unsuccessful, error, answer);
    }

    public class MMBotResult : RuntimeResult
    {
        private MMBotResult(CommandError? error, string reason = null, IMessage answer = null) : base(error, reason)
            => AnswerSent = answer;

        public IMessage AnswerSent { get; set; }

        public static MMBotResult Create(CommandError? error, string reason = null, IMessage answer = null) 
            => new(error, reason, answer);
    }
}
