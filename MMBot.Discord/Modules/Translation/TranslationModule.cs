using Discord.Commands;
using MMBot.Data.Contracts;
using MMBot.Discord.Modules.Interfaces;
using MMBot.Discord.Services.Interfaces;

namespace MMBot.Discord.Modules.Translation;

[Name("Translation")]
//[Group("Translation")]
[Alias("tr")]
public partial class TranslationModule : MMBotModule, ITranslationModule
{
    protected readonly ITranslationService _translationService;

    public TranslationModule(IDatabaseService databaseService, ICommandHandler commandHandler, ITranslationService translationService, IGuildSettingsService guildSettings)
        : base(databaseService, guildSettings, commandHandler)
    {
        _translationService = translationService;
    }

    [Command]
    [Summary("Translates any text to english")]
    public async Task<RuntimeResult> Translate([Remainder] string text)
    {
        var trans = await _translationService.TranslateTextAsync(text);

        if (string.IsNullOrWhiteSpace(trans) || trans.Contains("️"))
            return FromErrorUnsuccessful("Nothing to translate here.");

        var result = $"{Context.User.Mention} your translation: ```{trans}```";
        return FromSuccess(result);
    }
}
