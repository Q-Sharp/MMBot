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
using MMBot.Discord.Services.Interfaces;
using MMBot.Discord.Modules;
using MMBot.Data.Services.Interfaces;

namespace MMBot.Discord.Services.CommandHandler
{
    public partial class CommandHandler : ICommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IServiceProvider _services;
        private readonly ILogger<CommandHandler> _logger;
        private readonly IList<Tuple<ISocketMessageChannel, ISocketMessageChannel>> _formsChannelList = new List<Tuple<ISocketMessageChannel, ISocketMessageChannel>>();
        private ISocketMessageChannel _deletedMessagesChannel;
        
        private IGuildSettingsService _gs;
        private IDatabaseService _databaseService;
        private IAdminService _adminService;
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
            _adminService = _services.GetService<IAdminService>();

            _commands.CommandExecuted += CommandExecutedAsync;
            _commands.Log += LogAsync;

            _client.MessageReceived += Client_HandleCommandAsync;
            _client.ReactionAdded += Client_ReactionAdded;         
            _client.Ready += Client_Ready;
            _client.Log += LogAsync;
            _client.Disconnected += Client_Disconnected;

            _client.MessageDeleted += Client_MessageDeleted;
            _client.MessagesBulkDeleted += Client_MessagesBulkDeleted;
        }

        public void SetDeletedMessagesChannel(IGuildChannel channel) => _deletedMessagesChannel = channel as ISocketMessageChannel;

        private Task Client_MessagesBulkDeleted(IReadOnlyCollection<Cacheable<IMessage, ulong>> arg1, ISocketMessageChannel arg2)
        {
            arg1.ForEach(async x => await Client_MessageDeleted(x, arg2));
            return Task.CompletedTask;
        }

        private async Task Client_MessageDeleted(Cacheable<IMessage, ulong> arg1, ISocketMessageChannel arg2)
        {
            if(_deletedMessagesChannel is null)
                return;

            var m = await arg1.DownloadAsync() ?? arg1.Value;
            await _deletedMessagesChannel.SendMessageAsync($"{m.Author.GetUserAndDiscriminator()} deleted in {m.Channel.Name}: {m.Content}");
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
            try
            {
                _logger.Log(LogLevel.Information, "Starting CleanUp");
                await _databaseService.CleanDB(_client.Guilds.Select(g => Tuple.Create(g.Id, g.Name)));
            }
            catch(Exception e)
            {
                _logger.Log(LogLevel.Error, e, "Error in cleanup");
            }
            _logger.Log(LogLevel.Information, "CleanUp finished!");

            // handle restart information
            var r = await _databaseService.ConsumeRestart();
            if (r is not null)
            {
                var dest = _client.GetGuild(r.Guild).GetTextChannel(r.Channel);
                
                if(r.DBImport)
                    await dest.SendMessageAsync("Database imported!");
                else
                    await dest.SendMessageAsync("Bot service has been restarted!");
            }
               
            // load channels for google forms scan
            await ReInitGoogleFormsAsync();

            // reinit timers
            (await _databaseService.LoadTimerAsync())?
                .Where(x => x.IsActive && _timerService.Check(x))
                .ForEach(x => _timerService.Start(x, true));

            // set status
            await _client.SetGameAsync($"Member Manager 2020");
        }

        public async Task Client_HandleCommandAsync(SocketMessage arg)
        {
            if(arg.Author.IsWebhook && arg.Author is SocketWebhookUser whu)
                CheckForUseableLinks(arg, whu.Guild.Id);

            if (arg is not SocketUserMessage msg)
                return;

            var context = new SocketCommandContext(_client, msg);
            CheckForUseableLinks(arg, context.Guild.Id);

            if (msg.Author.Id == _client.CurrentUser.Id || msg.Author.IsBot) 
                return;

            var settings = await _gs.GetGuildSettingsAsync(context.Guild.Id);
            var pos = 0;

            if (msg.HasStringPrefix(settings.Prefix, ref pos) || msg.HasMentionPrefix(_client.CurrentUser, ref pos) || msg.Content.ToLower().StartsWith(settings.Prefix.ToLower()))
            {
                await _commands.ExecuteAsync(context, pos, _services);
                _ = Task.Delay(1000).ContinueWith(c => arg?.DeleteAsync(new RequestOptions { AuditLogReason = "Autoremoved" }));
            }
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

        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            // error happened
            if(!command.IsSpecified)
            {
                await context.Channel.SendMessageAsync($"I don't know this command: {context.Message}");
                return;
            }

            if (result is MMBotResult runTimeResult)
            {
                if (result.IsSuccess)
                {
                    if(runTimeResult.Reason is not null)
                        await context.Channel.SendMessageAsync(runTimeResult.Reason);
                    return;
                }

                //switch (runTimeResult?.Error ?? 0)
                //{
                //    case CommandError.BadArgCount:
                //        await context.Channel.SendMessageAsync(runTimeResult.Reason);
                //        break;

                //    case CommandError.Exception:
                //        await context.Channel.SendMessageAsync(runTimeResult.Reason);
                //        break;

                //    case CommandError.MultipleMatches:
                //        await context.Channel.SendMessageAsync(runTimeResult.Reason);
                //        break;

                //    case CommandError.ObjectNotFound:
                //        await context.Channel.SendMessageAsync(runTimeResult.Reason);
                //        break;

                //    case CommandError.ParseFailed:
                //        await context.Channel.SendMessageAsync(runTimeResult.Reason);
                //        break;

                //    case CommandError.UnknownCommand:
                //        await context.Channel.SendMessageAsync(runTimeResult.Reason);
                //        break;

                //    case CommandError.UnmetPrecondition:
                //        await context.Channel.SendMessageAsync(runTimeResult.Reason);
                //        break;

                //    case CommandError.Unsuccessful:
                //        await context.Channel.SendMessageAsync(runTimeResult.Reason);
                //        break;
                //}

                await context.Channel.SendMessageAsync($"{runTimeResult.Error}: {runTimeResult.Reason}");

                var member = context.User.GetUserAndDiscriminator();
                var moduleName = command.Value.Module.Name;
                var commandName = command.Value.Name;

                _logger.LogError($"{member} tried to use {commandName} (module: {moduleName}) this resultet in a {runTimeResult?.Error?.ToString()}");
            }
        }
    }
}
