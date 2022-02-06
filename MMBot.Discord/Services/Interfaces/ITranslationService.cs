namespace MMBot.Discord.Services.Interfaces;

public interface ITranslationService
{
    Task<string> TranslateTextAsync(string input);
}
