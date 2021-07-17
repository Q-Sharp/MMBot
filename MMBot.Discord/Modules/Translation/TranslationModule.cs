using Discord.Commands;
using System.Threading.Tasks;
using MMBot.Discord.Modules.Interfaces;
using MMBot.Discord.Services.Interfaces;
using MMBot.Data.Services.Interfaces;
using System;

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
        public async Task<RuntimeResult> CreateTimer([Remainder] string text)
        {
            var t = await _translationService.TranslateTextAsync(text);
             
            return FromSuccess($"{Context.User.Mention} translated ```{text}``` to ```{t}```");
        }
    }
}
