using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace TTMMBot.Services
{
    public class CommandHandler
    {
        private volatile IDictionary<ulong, Func<IEmote, Task>> messageIdWithReaction = new Dictionary<ulong, Func<IEmote, Task>>();

        public DiscordSocketClient Client { get; set; }
        public CommandService Commands { get; set; }
        public IServiceProvider Services { get; set; }

        public CommandHandler(IServiceProvider services, CommandService commands, DiscordSocketClient client)
        {
            Commands = commands;
            Services = services;
            Client = client;
        }

        public async Task InitializeAsync()
        {
            await Commands.AddModulesAsync(Assembly.GetEntryAssembly(), Services);
            Commands.CommandExecuted += CommandExecutedAsync;
            Client.MessageReceived += HandleCommandAsync;
            Client.ReactionAdded += Client_ReactionAdded;
        }

        private async Task Client_ReactionAdded(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
        {
            if (messageIdWithReaction.Keys.Contains(arg3.MessageId) && arg3.UserId != Client.CurrentUser.Id)
                await messageIdWithReaction[arg3.MessageId](arg3.Emote);
        }

        public async Task HandleCommandAsync(SocketMessage arg)
        {
            if (!(arg is SocketUserMessage msg) || msg.Author.Id == Client.CurrentUser.Id || msg.Author.IsBot) return;

            int pos = 0;
            if (msg.HasStringPrefix("m.", ref pos) || msg.HasMentionPrefix(Client.CurrentUser, ref pos))
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
            messageIdWithReaction.Add(message.Id, fT);
            Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromMinutes(1));
                messageIdWithReaction.Remove(message.Id);
            });
        }
    }
}
