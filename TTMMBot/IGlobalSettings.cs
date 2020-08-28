using System;

namespace TTMMBot
{
    public interface IGlobalSettings
    {
        string Prefix { get; set; }
        TimeSpan WaitForReaction { get; set; }
        bool UseTriggers { get; set; }
        string FileName { get; set; }
        int ClanSize { get; set; }
    }
}