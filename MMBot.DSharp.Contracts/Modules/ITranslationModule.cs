namespace MMBot.DSharp.Contracts.Modules;

public interface ITranslationModule
{
    Task Translate(string text);
}
