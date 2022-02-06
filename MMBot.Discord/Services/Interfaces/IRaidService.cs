namespace MMBot.Discord.Services.Interfaces;

public interface IRaidService
{
    Task ConnectAsync();
    Task<byte[]> GetTacticPicture();
}
