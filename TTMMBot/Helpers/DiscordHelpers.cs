using Discord.WebSocket;

namespace TTMMBot.Helpers
{
    public static class DiscordHelpers
    {
        public static string GetUserAndDiscriminator(this SocketGuildUser sgu) => $"{sgu.Username}#{sgu.Discriminator}";
        public static string GetUserAndDiscriminator(this SocketUser sgu) => $"{sgu.Username}#{sgu.Discriminator}";
    }
}
