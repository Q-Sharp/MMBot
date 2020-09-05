using System;
using TTMMBot.Data;

namespace TTMMBot.Services
{
    public interface IGlobalSettingsService
    {
        Context Dbcontext { get; set; }
        string Prefix { get; }
        TimeSpan WaitForReaction { get; }
        bool? UseTriggers { get; set; }
        string FileName { get; }
        int ClanSize { get; }
    }
}
