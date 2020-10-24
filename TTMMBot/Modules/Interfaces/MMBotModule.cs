using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using TTMMBot.Helpers;
using TTMMBot.Services.Interfaces;

namespace TTMMBot.Modules.Interfaces
{
    public abstract class MMBotModule : ModuleBase<SocketCommandContext>
    {
        protected readonly IDatabaseService _databaseService;
        protected readonly ICommandHandler _commandHandler;
        protected readonly IGuildSettingsService _guildSettings;

        public MMBotModule(IDatabaseService databaseService, IGuildSettingsService guildSettings, ICommandHandler commandHandler)
        {
            _databaseService = databaseService;
            _commandHandler = commandHandler;
            _guildSettings = guildSettings;
        }

        protected override void BeforeExecute(CommandInfo command)
        {
            _databaseService.SetGuild(Context.Channel.Id);
            _guildSettings.LoadSettings(Context.Channel.Id);
            base.BeforeExecute(command);
        }
    }
}
