using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TTMMBot.Helpers;
using TTMMBot.Services.Interfaces;

namespace TTMMBot.Services
{
    public class CommandHandler : ICommandHandler
    {
        private readonly IDictionary<ulong, Func<IEmote, IUser, Task>> _messageIdWithReaction = new Dictionary<ulong, Func<IEmote, IUser, Task>>();
        private readonly IList<ISocketMessageChannel> _channelList = new List<ISocketMessageChannel>();

        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IServiceProvider _services;
        private readonly IGlobalSettingsService _gs;
        private readonly IDatabaseService _databaseService;
        private readonly IGoogleFormsService _googleFormsSubmissionService;
        private readonly ILogger<CommandHandler> _logger;

        public CommandHandler(IServiceProvider services, CommandService commands, DiscordSocketClient client, GlobalSettingsService gs, ILogger<CommandHandler> logger, IDatabaseService databaseService, IGoogleFormsService gfss)
        {
            _commands = commands;
            _services = services;
            _client = client;
            _gs = gs;
            _logger = logger;
            _databaseService = databaseService;
            _googleFormsSubmissionService = gfss;
        }

        public async Task InitializeAsync()
        {
            await _commands.AddModulesAsync(GetType().Assembly, _services);

            _commands.CommandExecuted += CommandExecutedAsync;

            _client.MessageReceived += HandleCommandAsync;
            _client.ReactionAdded += Client_ReactionAdded;
            _client.Disconnected += Client_Disconnected;

            _client.Ready += Client_Ready;
        }

        private async Task Client_Ready()
        {
            var r = await _databaseService.ConsumeRestart();
            _logger.Log(LogLevel.Information, "Bot is connected!");

            if (r != null)
                await _client.GetGuild(r.Item1)
                    .GetTextChannel(r.Item2)
                    .SendMessageAsync("Bot service has been restarted!");

            await _databaseService.CleanDB();

            //(await DatabaseService.LoadChannelsAsync())?
            //    .Select(x => Client.GetGuild(x.GuildId).GetTextChannel(x.TextChannelId))?
            //    .ForEach(x => _channelList.Add(x));

            //await GoogleFormsSubmissionService.SubmitAsync("https://docs.google.com/forms/d/e/1FAIpQLSc1Rc_aCu4lEO9EWNCtjOlLF9FQJz47oVmi2ka2ncjs1yzMrg/viewform?usp=sf_link", "");
        }

        private async Task Client_Disconnected(Exception arg)
        {
            _logger.LogError(arg.Message);

            await Task.Run(() =>
            { 
                Process.Start(AppDomain.CurrentDomain.FriendlyName);
                Environment.Exit(0);
            });
        }

        private async Task Client_ReactionAdded(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
            => await Task.Run(() =>
            {
                lock (_messageIdWithReaction)
                {
                    if (_messageIdWithReaction.Keys.Contains(arg3.MessageId) && arg3.UserId != _client.CurrentUser.Id)
                        _messageIdWithReaction[arg3.MessageId](arg3.Emote, arg3.User.IsSpecified ? arg3.User.Value : null);
                }
            });

        public async Task HandleCommandAsync(SocketMessage arg)
        {
            if (_channelList.Contains(arg.Channel))
            {
                var urls = arg.Content.GetUrl();
                urls.ForEach(async x =>
                {
                    var m = await _databaseService.LoadMembersAsync();

                    m.Where(z => z.AutoSignUpForFightNight && z.PlayerTag != null).ForEach(async y => await _googleFormsSubmissionService.SubmitAsync(x, "@Tag"));
                });
            }

            if (!(arg is SocketUserMessage msg) || msg.Author.Id == _client.CurrentUser.Id || msg.Author.IsBot) return;

            var pos = 0;
            if (msg.HasStringPrefix(_gs.Prefix, ref pos) || msg.HasMentionPrefix(_client.CurrentUser, ref pos))
            {
                var context = new SocketCommandContext(_client, msg);
                var result = await _commands.ExecuteAsync(context, pos, _services);

                if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                    await msg.Channel.SendMessageAsync(result.ErrorReason);
            }
        }

        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (!command.IsSpecified || result.IsSuccess)
                return;

            await context.Channel.SendMessageAsync($"error: {result}");
        }

        public Task AddToReactionList(IUserMessage message, Func<IEmote, IUser, Task> fT)
        {
            lock (_messageIdWithReaction)
            {
                _messageIdWithReaction.Add(message.Id, fT);
            }

            return Task.Delay(_gs.WaitForReaction)
                .ContinueWith(x =>
                {
                    lock (_messageIdWithReaction)
                    {
                        _messageIdWithReaction.Remove(message.Id);
                    }
                });
        }

        public Task AddChannelToGoogleFormsWatchList(IGuildChannel channel)
        {
            lock (_messageIdWithReaction)
            {
                _channelList.Add(channel as ISocketMessageChannel);
            }

            return Task.Run(async () => 
            {
                var c = await _databaseService.CreateChannelAsync();
                c.TextChannelId = channel.Id;
                c.GuildId = channel.GuildId;
                await _databaseService.SaveDataAsync();
            });
        }

        public Task RemoveChannelFromGoogleFormsWatchList(IGuildChannel channel)
        {
            lock (_messageIdWithReaction)
            {
                _channelList.Remove(channel as ISocketMessageChannel);
            }

            return Task.Run(async () => 
            {
                var c = (await _databaseService.LoadChannelsAsync()).FirstOrDefault(x => x.GuildId == channel.GuildId && x.TextChannelId == channel.Id);

                if(c != null)
                    _databaseService.DeleteChannel(c);

                await _databaseService.SaveDataAsync();
            });
        }
    }
}
