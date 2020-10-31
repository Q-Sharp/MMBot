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
        Task DeleteTimer(string name);
        Task StartTimer(string name, string timeToFirstRing, string timeInterval = null, double? offSet = null);
        Task StopTimer(string name);
        Task AddNotification(string name, ISocketMessageChannel channel, [Remainder] string message);
        Task RemoveNotification(string name);
    }
}