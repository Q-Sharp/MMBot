using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace TTMMBot.Services
{
    public interface ICommandHandler
    {
        DiscordSocketClient Client { get; set; }
        CommandService Commands { get; set; }
        IServiceProvider Services { get; set; }
        GlobalSettings Gs { get; set; }
        Task InitializeAsync();
        Task HandleCommandAsync(SocketMessage arg);
        Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result);
        Task SaveRestartInformationAsync(string channelOfRestartCommand);
        void AddToReactionList(IUserMessage message, Func<IEmote, Task> fT);
    }
}