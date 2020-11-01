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

            _client.MessageReceived += Client_HandleCommandAsync;
            _client.ReactionAdded += Client_ReactionAdded;
            _client.Disconnected += Client_Disconnected;

            _client.Ready += Client_Ready;
        }

        private async Task Client_Ready()
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

        private async Task Client_Disconnected(Exception arg)
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
            CheckGoogleForms(arg);

            if (!(arg is SocketUserMessage msg) || msg.Author.Id == _client.CurrentUser.Id || msg.Author.IsBot) return;
            _gs?.SetGuild((msg.Channel as IGuildChannel).Id);

            var pos = 0;
            if (msg.HasStringPrefix(_gs.Prefix, ref pos) || msg.HasMentionPrefix(_client.CurrentUser, ref pos) || msg.Content.ToLower().StartsWith(_gs.Prefix.ToLower()))
            {
                var context = new SocketCommandContext(_client, msg);
                var result = await _commands.ExecuteAsync(context, pos, _services);

                if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                    await msg.Channel.SendMessageAsync(result.ErrorReason);
            }
        }

        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (result.IsSuccess)
                return;

            if(!command.IsSpecified)
            {
                await context.Channel.SendMessageAsync($"I don't know this command: {context.Message}");
                return;
            }
                
            await context.Channel.SendMessageAsync($"error: {result}");
        }
    }
}
