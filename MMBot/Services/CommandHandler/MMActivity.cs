using System;
using System.Collections.Generic;
using System.Text;
using Discord;

namespace MMBot.Services.CommandHandler
{
    public class MMActivity : IActivity
    {
        public string Name => "Manage Members";

        public ActivityType Type => ActivityType.Watching;

        public ActivityProperties Flags => ActivityProperties.Spectate | ActivityProperties.Instance;

        public string Details => "No details here.";
    }
}
