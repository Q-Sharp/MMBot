using System.Reflection;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using MMBot.Helpers;
using MMBot.Services.Interfaces;

namespace MMBot.Modules
{
    public abstract class MMBotModule : ModuleBase<SocketCommandContext>
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

        protected override void BeforeExecute(CommandInfo command)
        {
            //typeof(MMBotModule).GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField)
            //    .ForEach(x => (x.GetValue(this) as IGuildSetter)?.SetGuild(Context.Guild.Id));

            //base.BeforeExecute(command);
        }
    }
}
