namespace MMBot.DSharp.Contracts.Services;

public interface IRaidService
{
    Task ConnectAsync();
    Task<byte[]> GetTacticPicture();
}
