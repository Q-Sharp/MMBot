using MMBot.Data.Contracts.Entities;

namespace MMBot.DSharp.Contracts.Services;

public interface ITimerService
{
    Task Start(MMTimer t, bool bReinit = false, TimeSpan? ToFirstRing = null);
    Task Stop(MMTimer t);
    bool Check(MMTimer t);
    Task<string> GetCountDown(MMTimer t);
}
