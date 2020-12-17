using Discord.WebSocket;
using MMBot.Data.Entities;

namespace MMBot.Discord.Services.Timer
{
    public class TimerContainer
    {
        public System.Threading.Timer Timer { get; set; }
        public MMTimer TimerInfos { get; set; }
        public DiscordSocketClient SocketClient { get; set; }
        public bool IsTemp { get; set; }
    }
}
