namespace MMBot.DSharp.Contracts.Services;

public interface IGoogleSheetsService
{
    Task ConnectAsync();

    Task<byte[]> GetTacticPictureAsync();
}
