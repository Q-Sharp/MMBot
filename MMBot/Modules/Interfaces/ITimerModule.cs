using System.Threading.Tasks;

namespace MMBot.Modules.Interfaces
{
    public interface ITimerModule
    {
        Task CreateTimer(string name, bool recurring);
        Task ListTimers();
        Task DeleteTimer(string name);
        Task StartTimer(string name, string timeToFirstRing, string timeInterval = null, double? offSet = null);
        Task StopTimer(string name);
        Task ShowTimeLeft(string name);
    }
}