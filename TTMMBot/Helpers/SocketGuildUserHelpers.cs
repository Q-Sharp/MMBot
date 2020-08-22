using Discord.WebSocket;

namespace TTMMBot.Helpers
{
    public static class SocketGuildUserHelpers
    {
        public static string GetUserAndDiscriminator(this SocketGuildUser sgu) => $"{sgu.Username}#{sgu.Discriminator}";
    }
}
