﻿namespace MMBot.Discord.Services.Interfaces;

public interface IGoogleSheetsService
{
    Task ConnectAsync();

    Task<byte[]> GetTacticPictureAsync();
}
