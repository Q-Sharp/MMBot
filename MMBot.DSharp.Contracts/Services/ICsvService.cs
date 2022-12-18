namespace MMBot.DSharp.Contracts.Services;

public interface ICsvService
{
    Task<Exception> ImportCsv(byte[] csv, ulong guildID);
    Task<byte[]> ExportCsv(ulong guildID);
}
