using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

namespace MMBot.Modules.Interfaces
{
    public interface ITimerModule
    {
        Task CreateTimer(string name, bool recurring);
        Task ListTimers();
        Task DeleteTimer(string Name);
        Task StartTimer(string name, [Remainder] string timeSpan);
        Task StopTimer(string name);
        Task AddNotification(string name, ISocketMessageChannel channel, [Remainder] string message);
        Task RemoveNotification(string name);
    }
}