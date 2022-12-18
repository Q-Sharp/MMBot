namespace MMBot.DSharp.Contracts.Services;

public interface ITranslationService
{
    Task<string> TranslateTextAsync(string input);
}
