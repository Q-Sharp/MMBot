using System;
using System.Collections.Generic;
using System.Text;

namespace TTMMBot
{
    public class GlobalSettings
    {
        public string Prefix { get; set; } = "m.";

        public bool UseTriggers { get; set; } = true;

        public TimeSpan WaitForReaction { get; set; } = TimeSpan.FromMinutes(5);
    }
}
