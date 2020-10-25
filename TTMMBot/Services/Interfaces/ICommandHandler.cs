using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace TTMMBot.Services.Interfaces
{
    public interface ICommandHandler : IMMBotInterface 
    {
        Task InitializeAsync();
        Task Client_HandleCommandAsync(SocketMessage arg);
        Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result);
        Task AddToReactionList(IUserMessage message, Func<IEmote, IUser, Task> fT, bool allowMultiple = true);
        Task AddChannelToGoogleFormsWatchList(IGuildChannel channel);
        Task RemoveChannelFromGoogleFormsWatchList(IGuildChannel channel);
    }
}
