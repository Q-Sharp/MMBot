﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Discord.Commands;
using Discord.WebSocket;
using TTMMBot.Helpers;
using TTMMBot.Services.Interfaces;

namespace TTMMBot.Modules
{
    public abstract class MMBotModule : ModuleBase<CommandContext>
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
            typeof(MMBotModule)
                .GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField)
                .OfType<IGuildSetter>()
                .ForEach(s => s.SetGuild(Context.Channel.Id));

            base.BeforeExecute(command);
        }
    }
}
