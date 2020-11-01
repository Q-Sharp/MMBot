using Discord.Commands;

namespace MMBot.Modules
{
    public class MMBotResult : RuntimeResult
    {

    // #TODO: IMPLEMENT Task<RuntimeResult> as return type for all commands for better error handling
        private MMBotResult(CommandError? error, string reason = null) : base(error, reason)
        {
        }

        public static MMBotResult Create(CommandError? error, string reason= null) 
            => new MMBotResult(error, reason);

        public static MMBotResult FromSuccess(string reason = null) 
            => MMBotResult.Create(null, reason);

        public static MMBotResult FromError(string reason) 
            => MMBotResult.Create(CommandError.Unsuccessful, reason);

        public static MMBotResult FromErrorObjectNotFound(string reason) 
            => MMBotResult.Create(CommandError.ObjectNotFound, reason);
    }
}
