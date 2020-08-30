using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using TTMMBot.Modules;

namespace TTMMBot.Services
{
    public class CommandHandler : ICommandHandler
    {
        private volatile IDictionary<ulong, Func<IEmote, Task>> _messageIdWithReaction = new Dictionary<ulong, Func<IEmote, Task>>();

        public DiscordSocketClient Client { get; set; }
        public CommandService Commands { get; set; }
        public IServiceProvider Services { get; set; }
        public GlobalSettings Gs { get; set; }

        public ILogger<CommandHandler> Logger { get; set; }

        public CommandHandler(IServiceProvider services, CommandService commands, DiscordSocketClient client, GlobalSettings gs, ILogger<CommandHandler> logger)
        {
            Commands = commands;
            Services = services;
            Client = client;
            Gs = gs;
            Logger = logger;
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

            Client.Ready += () =>
            {
                var r = LoadRestartInformationAsync();

                if (r is null)
                    Logger.Log(LogLevel.Information, "Bot is connected!");
                else
                    Client.GroupChannels.FirstOrDefault(x => x.Name == r)?.SendMessageAsync("Bot service is restarted!");

                return Task.CompletedTask;
            };
        }

        private async Task Client_ReactionAdded(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
        {
            if (_messageIdWithReaction.Keys.Contains(arg3.MessageId) && arg3.UserId != Client.CurrentUser.Id)
                await _messageIdWithReaction[arg3.MessageId](arg3.Emote);
        }

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

        public void AddToReactionList(IUserMessage message, Func<IEmote, Task> fT)
        {
            _messageIdWithReaction.Add(message.Id, fT);
            Task.Run(async () =>
            {
                await Task.Delay(Gs.WaitForReaction);
                _messageIdWithReaction.Remove(message.Id);
            });
        }

        public async Task SaveRestartInformationAsync(string channelOfRestartCommand)
        {
            try
            {
                await File.WriteAllTextAsync("restart", channelOfRestartCommand);
            }
            catch(Exception e)
            { 
                Logger.Log(LogLevel.Error, e.Message);
            }
        }

        public string LoadRestartInformationAsync()
        {
            try
            {
                var channel = File.ReadAllText("restart");
                File.Delete("restart");
                return channel;
            }
            catch(Exception e)
            { 
                Logger.Log(LogLevel.Error, e.Message);
                return null;
            }
        }
    }
}
