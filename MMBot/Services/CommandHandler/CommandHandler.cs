using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MMBot.Helpers;
using MMBot.Services.Interfaces;

namespace MMBot.Services.CommandHandler
{
    public partial class CommandHandler : ICommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IServiceProvider _services;
        private readonly ILogger<CommandHandler> _logger;
        private readonly IList<Tuple<ISocketMessageChannel, ISocketMessageChannel>> _formsChannelList = new List<Tuple<ISocketMessageChannel, ISocketMessageChannel>>();
        
        private IGuildSettingsService _gs;
        private IDatabaseService _databaseService;
        private IGoogleFormsService _googleFormsSubmissionService;
        private InteractiveService _interactiveService;
        private ITimerService _timerService;

        public CommandHandler(IServiceProvider services, CommandService commands, DiscordSocketClient client, ILogger<CommandHandler> logger)
        {
            _commands = commands;
            _services = services;
            _client = client;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            await _commands.AddModulesAsync(GetType().Assembly, _services);

            _databaseService = _services.GetService<IDatabaseService>();
            _gs = _services.GetService<IGuildSettingsService>();
            _googleFormsSubmissionService = _services.GetService<IGoogleFormsService>();
            _interactiveService = _services.GetService<InteractiveService>();
            _timerService = _services.GetService<ITimerService>();

            _commands.CommandExecuted += CommandExecutedAsync;
            _commands.Log += LogAsync;

            _client.MessageReceived += Client_HandleCommandAsync;
            _client.ReactionAdded += Client_ReactionAdded;
            _client.Disconnected += Client_Disconnected;

            _client.Ready += Client_Ready;
        }

        public async Task LogAsync(LogMessage logMessage)
        {
            if (logMessage.Exception is CommandException cmdException)
            {
                await cmdException.Context.Channel.SendMessageAsync("Something went catastrophically wrong!");
                _logger.LogError($"{cmdException.Context.User} failed to execute '{cmdException.Command.Name}' in {cmdException.Context.Channel}.", logMessage.Exception);
            }
        }

        public async Task Client_Ready()
        {
            _logger.Log(LogLevel.Information, "Bot is connected!");

            // clean db if needed
            await _databaseService.CleanDB();

            // handle restart information
            var r = await _databaseService.ConsumeRestart();
            if (r != null)
                await _client.GetGuild(r.Item1)
                    .GetTextChannel(r.Item2)
                    .SendMessageAsync("Bot service has been restarted!");

            // load channels for google forms scan
            await ReInitGoogleFormsAsync();

            // reinit timers
            (await _databaseService.LoadTimerAsync())?
                .Where(x => x.IsActive && _timerService.Check(x))
                .ForEach(x => _timerService.Start(x, true));

            // set status
            await _client.SetGameAsync($"Member Manager 2020");
        }

        public async Task Client_Disconnected(Exception arg)
        {
            _logger.LogError(arg.Message, arg);

            await Task.Run(() =>
            { 
                Process.Start(AppDomain.CurrentDomain.FriendlyName);
                Environment.Exit(0);
            });
        }

        public async Task Client_HandleCommandAsync(SocketMessage arg)
        {
            if (!(arg is SocketUserMessage msg))
                return;

            var context = new SocketCommandContext(_client, msg);

            CheckGoogleForms(arg, context.Guild.Id);

            if (msg.Author.Id == _client.CurrentUser.Id || msg.Author.IsBot) 
                return;

            var settings = await _gs.GetGuildSettingsAsync(context.Guild.Id);
            var pos = 0;
            if (msg.HasStringPrefix(settings.Prefix, ref pos) || msg.HasMentionPrefix(_client.CurrentUser, ref pos) || msg.Content.ToLower().StartsWith(settings.Prefix.ToLower()))
            {
                
                await _commands.ExecuteAsync(context, pos, _services);
            }
        }

        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (result.IsSuccess)
                return;

            // error happened
            if(!command.IsSpecified)
            {
                await context.Channel.SendMessageAsync($"I don't know this command: {context.Message}");
                return;
            }

            //#TODO:
            //var moduleName = command.Value.Module.Name;
            //var commandName = command.Value.Name;

            //if(result is MMBotResult runTimeResult)
            //{
            //    switch(runTimeResult?.Error ?? 0)
            //    {
            //        case CommandError.BadArgCount:
            //            break;

            //        case CommandError.Exception:
            //            break;

            //        case CommandError.MultipleMatches:
            //            break;

            //        case CommandError.ObjectNotFound:
            //            break;
                    
            //        case CommandError.ParseFailed:
            //            break;

            //        case CommandError.UnknownCommand:
            //            break;

            //        case CommandError.UnmetPrecondition:
            //            break;

            //        case CommandError.Unsuccessful:
            //            break;
            //    }
            //}
   
            await context.Channel.SendMessageAsync($"error: {result.ErrorReason}");
        }
    }
}
