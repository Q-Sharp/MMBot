using Discord;
using Discord.WebSocket;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MMBot.Services.Interfaces;

namespace MMBot.Services.CommandHandler
{
    public partial class CommandHandler : ICommandHandler
    {
        private readonly IDictionary<ulong, Func<IEmote, IUser, Task>> _messageIdWithReaction = new Dictionary<ulong, Func<IEmote, IUser, Task>>();
        private readonly AsyncLock _mutex = new AsyncLock();
        private readonly AsyncMonitor _monitor = new AsyncMonitor();

        private async Task Client_ReactionAdded(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
            => await Task.Run(async () =>
            {
                using (await _mutex.LockAsync())
                {
                    if (_messageIdWithReaction.Keys.Contains(arg3.MessageId) && arg3.UserId != _client.CurrentUser.Id)
                    {
                        await _messageIdWithReaction[arg3.MessageId](arg3.Emote, arg3.User.IsSpecified ? arg3.User.Value : null);
                        _monitor.PulseAll();
                    }   
                }
            });

        public async Task AddToReactionList(ulong guildId,IUserMessage message, Func<IEmote, IUser, Task> fT, bool allowMultiple = true)
        {
            var settings = await _gs.GetGuildSettingsAsync(guildId);

            using (await _mutex.LockAsync())
                _messageIdWithReaction.Add(message.Id, fT);

            var _ = await _monitor.EnterAsync();

            await Task.Run(async () => 
            {
                var t1 = Task.Delay(settings.WaitForReaction);
                var t2 = allowMultiple ? Task.Delay(-1) : _monitor.WaitAsync();

                await Task.WhenAny(t1, t2);   
            })
            .ContinueWith(async x =>
            {
                using (await _mutex.LockAsync())
                    _messageIdWithReaction.Remove(message.Id);

                _.Dispose();
            });
        }
    }
}
