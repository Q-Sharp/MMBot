using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using TTMMBot.Modules;
using TTMMBot.Services.Interfaces;

namespace TTMMBot.Services
{
    public class CommandHandler : ICommandHandler
    {
        private readonly IDictionary<ulong, Func<IEmote, IUser, Task>> _messageIdWithReaction = new Dictionary<ulong, Func<IEmote, IUser, Task>>();

        public DiscordSocketClient Client { get; set; }
        public CommandService Commands { get; set; }
        public IServiceProvider Services { get; set; }
        public IGlobalSettingsService Gs { get; set; }
        public IDatabaseService DatabaseService { get; set; }

        public ILogger<CommandHandler> Logger { get; set; }

        public CommandHandler(IServiceProvider services, CommandService commands, DiscordSocketClient client, GlobalSettingsService gs, ILogger<CommandHandler> logger, IDatabaseService databaseService)
        {
            Commands = commands;
            Services = services;
            Client = client;
            Gs = gs;
            Logger = logger;
            DatabaseService = databaseService;
        }

        public async Task InitializeAsync()
        {
            await Commands.AddModuleAsync<ClanModule>(Services);
            await Commands.AddModuleAsync<MemberModule>(Services);
            await Commands.AddModuleAsync<HelpModule>(Services);
            await Commands.AddModuleAsync<AdminModule>(Services);

            Commands.CommandExecuted += CommandExecutedAsync;

            Client.MessageReceived += HandleCommandAsync;
            Client.ReactionAdded += Client_ReactionAdded;
            Client.Disconnected += Client_Disconnected;

            Client.Ready += Client_Ready;
        }

        private async Task Client_Ready()
        {
            var r = await DatabaseService.ConsumeRestart();

            if (r is null)
                Logger.Log(LogLevel.Information, "Bot is connected!");
            else
                await Client.GetGuild(r.Item1).GetTextChannel(r.Item2).SendMessageAsync("Bot service has been restarted!");
        }

        private async Task Client_Disconnected(Exception arg)
        {
            Logger.LogError(arg.Message);

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
                    if (_messageIdWithReaction.Keys.Contains(arg3.MessageId) && arg3.UserId != Client.CurrentUser.Id)
                        _messageIdWithReaction[arg3.MessageId](arg3.Emote, arg3.User.IsSpecified ? arg3.User.Value : null);
                }
            });

        public async Task HandleCommandAsync(SocketMessage arg)
        {
            if (!(arg is SocketUserMessage msg) || msg.Author.Id == Client.CurrentUser.Id || msg.Author.IsBot) return;

            var pos = 0;
            if (msg.HasStringPrefix(Gs.Prefix, ref pos) || msg.HasMentionPrefix(Client.CurrentUser, ref pos))
            {
                var context = new SocketCommandContext(Client, msg);
                var result = await Commands.ExecuteAsync(context, pos, Services);

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

            return Task.Delay(Gs.WaitForReaction)
                .ContinueWith(x =>
                {
                    lock (_messageIdWithReaction)
                    {
                        _messageIdWithReaction.Remove(message.Id);
                    }
                });
        }
    }
}
