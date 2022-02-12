using Discord;
using Discord.WebSocket;
using MMBot.Discord.Services.Interfaces;
using Nito.AsyncEx;

namespace MMBot.Discord.Services.CommandHandler;

public partial class CommandHandler : ICommandHandler
{
    private readonly IDictionary<ulong, Func<IEmote, IUser, Task>> _messageIdWithReaction = new Dictionary<ulong, Func<IEmote, IUser, Task>>();
    private static readonly AsyncLock _mutex = new();
    private static readonly AsyncMonitor _monitor = new();

    private async Task Client_ReactionAdded(Cacheable<IUserMessage, ulong> arg1, Cacheable<IMessageChannel, ulong> arg2, SocketReaction arg3)
        => await Task.Run(async () =>
        {
            using (await _mutex.LockAsync())
            {
                if (_messageIdWithReaction.Keys.Contains(arg3.MessageId) && arg3.UserId != _client.CurrentUser.Id)
                {
                    await _messageIdWithReaction[arg3.MessageId](arg3.Emote, arg3.User.IsSpecified ? arg3.User.Value : null);
                    _monitor.Pulse();
                }
            }
        });

    public async Task AddToReactionList(ulong guildId, IUserMessage message, Func<IEmote, IUser, Task> fT, bool allowMultiple = true)
    {
        var settings = await _gs.GetGuildSettingsAsync(guildId);

        using (await _mutex.LockAsync())
            _messageIdWithReaction.Add(message.Id, fT);

        var dis = await _monitor.EnterAsync().ConfigureAwait(false);

        var t1 = Task.Delay(Data.Entities.GuildSettings.WaitForReaction);
        var t2 = allowMultiple ? Task.Delay(-1) : _monitor.WaitAsync();

        await Task.WhenAny(t1, t2).ConfigureAwait(false);

        using (await _mutex.LockAsync().ConfigureAwait(false))
            _messageIdWithReaction.Remove(message.Id);

        dis.Dispose();
    }
}
