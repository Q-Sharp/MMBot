namespace MMBot.Services.Interfaces
{
    public interface IGuildSetter : IMMBotInterface
    {
        void SetGuild(ulong id);
    }
}
