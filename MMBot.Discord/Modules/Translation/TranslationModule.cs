using Discord.Commands;
using System.Threading.Tasks;
using MMBot.Discord.Modules.Interfaces;
using MMBot.Discord.Services.Interfaces;
using MMBot.Data.Services.Interfaces;
using MMBot.Helpers;

namespace MMBot.Discord.Modules.Translation
{
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
            var t = await _translationService.TranslateTextAsync(text);

            var result = $"{Context.User.Mention} translated ```{DiscordHelpers.SeperateMention(text, out _)}``` to ```{t.Item1}```";

            if (t.Item2 != null)
                result += $" for {t.Item2}";

            return FromSuccess(result);
        }
    }
}
