using Discord.Commands;

namespace MMBot.Discord.Modules.Interfaces;

public interface ITimerModule
{
    Task<RuntimeResult> CreateTimer(string name, bool recurring);
    Task<RuntimeResult> ListTimers();
    Task<RuntimeResult> DeleteTimer(string name);
    Task<RuntimeResult> StartTimer(string name, string timeToFirstRing, string timeInterval = null);
    Task<RuntimeResult> StopTimer(string name);
    Task<RuntimeResult> ShowTimeLeft(string name);
}
