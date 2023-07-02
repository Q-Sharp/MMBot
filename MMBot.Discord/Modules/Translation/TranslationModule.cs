using Discord.Commands;
using MMBot.Data.Contracts;
using MMBot.Discord.Modules.Interfaces;
using MMBot.Discord.Services.Interfaces;

namespace MMBot.Discord.Modules.Translation;

[Name("Translation")]
//[Group("Translation")]
public partial class TranslationModule : MMBotModule, ITranslationModule
{
    protected readonly ITranslationService _translationService;

    public TranslationModule(IDatabaseService databaseService, ICommandHandler commandHandler, ITranslationService translationService, IGuildSettingsService guildSettings)
        : base(databaseService, guildSettings, commandHandler) => _translationService = translationService;

    [Command("translate")]
    [Summary("Translates any text to english")]
    [Alias("tr")]
    public async Task<RuntimeResult> Translate([Remainder] string text)
    {
        var trans = await _translationService.TranslateTextAsync(text);

        if (string.IsNullOrWhiteSpace(trans) || trans.Contains("️"))
            return FromErrorUnsuccessful("Nothing to translate here.");

        var result = $"{Context.User.Mention} your translation: ```{trans}```";
        return FromSuccess(result);
    }

    [Command("translateAdvance")]
    [Summary("Translates any text to any language")]
    [Alias("tra")]
    public async Task<RuntimeResult> Translate(string langcode, [Remainder] string text)
    {
        var trans = await _translationService.TranslateTextAsync(text, langcode);

        if (string.IsNullOrWhiteSpace(trans) || trans.Contains("️"))
            return FromErrorUnsuccessful("Nothing to translate here.");

        var result = $"{Context.User.Mention} your translation: ```{trans}```";
        return FromSuccess(result);
    }

    [Command("translateHelp")]
    [Summary("Get possible language codes")]
    [Alias("trh")]
    public async Task<RuntimeResult> GetLangCodes()
    {
        return await Task.Run(() => FromSuccess("https://cloud.google.com/translate/docs/languages"));
    }
}
