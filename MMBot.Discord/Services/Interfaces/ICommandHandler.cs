using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace MMBot.Discord.Services.Interfaces;

public interface ICommandHandler
{
    Task InitializeAsync();
    Task Client_HandleCommandAsync(SocketMessage arg);
    Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result);
    Task AddToReactionList(ulong guildId, IUserMessage message, Func<IEmote, IUser, Task> fT, bool allowMultiple = true);
}
