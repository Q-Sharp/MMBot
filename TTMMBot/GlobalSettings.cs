using System;
using System.Collections.Generic;
using System.Text;

namespace TTMMBot
{
    public class GlobalSettings
    {
        public bool UseTriggers { get; set; } = true;

        public TimeSpan WaitForReaction { get; set; } = TimeSpan.FromMinutes(5);
    }
}
