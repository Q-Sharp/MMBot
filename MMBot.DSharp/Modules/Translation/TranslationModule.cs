namespace MMBot.DSharp.Modules.Translation;

public partial class TranslationModule : MMBotModule
{
    protected readonly ITranslationService _translationService;

    public TranslationModule(IDatabaseService databaseService, ITranslationService translationService/*, IGuildSettingsService guildSettings, InteractionContext ctx*/)
        : base(databaseService/*, guildSettings, ctx*/) 
        => _translationService = translationService;

    [SlashCommand("translate", "Translates any text to english")]
    public async Task Translate(InteractionContext ctx, [Option("text", "text")] string text)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        var trans = await _translationService.TranslateTextAsync(text);

        await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent(trans));
    }
}
