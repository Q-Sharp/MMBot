namespace MMBot.DSharp;

public class SlashCommandHandler : ApplicationCommandModule
{
    [SlashCommand("test", "A slash command made to test the DSharpPlus Slash Commands extension!")]
    public async Task TestCommand(InteractionContext ctx)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Success!"));
    }

    [SlashCommand("delaytest", "A slash command made to test the DSharpPlus Slash Commands extension!")]
    public async Task DelayTestCommand(InteractionContext ctx)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        //Some time consuming task like a database call or a complex operation

        await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Thanks for waiting!"));
    }

    //Attribute choices
    [SlashCommand("ban", "Bans a user")]
    public async Task Ban(InteractionContext ctx, [Option("user", "User to ban")] DiscordUser user,
        [Choice("None", 0)]
    [Choice("1 Day", 1)]
    [Choice("1 Week", 7)]
    [Option("deletedays", "Number of days of message history to delete")] long deleteDays = 0)
    {
        await ctx.Guild.BanMemberAsync(user.Id, (int)deleteDays);
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Banned {user.Username}"));
    }

    //Enum choices
    public enum MyEnum
    {
        [ChoiceName("Option 1")]
        option1,
        [ChoiceName("Option 2")]
        option2,
        [ChoiceName("Option 3")]
        option3
    }

    [SlashCommand("enum", "Test enum")]
    public async Task EnumCommand(InteractionContext ctx, [Option("enum", "enum option")] MyEnum myEnum = MyEnum.option1)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent(myEnum.GetName()));
    }

    //ChoiceProvider choices
    public class TestChoiceProvider : IChoiceProvider
    {
        public async Task<IEnumerable<DiscordApplicationCommandOptionChoice>> Provider()
        {
            return new DiscordApplicationCommandOptionChoice[]
            {
            //You would normally use a database call here
            new DiscordApplicationCommandOptionChoice("testing", "testing"),
            new DiscordApplicationCommandOptionChoice("testing2", "test option 2")
            };
        }
    }

    [SlashCommand("choiceprovider", "test")]
    public async Task ChoiceProviderCommand(InteractionContext ctx,
        [ChoiceProvider(typeof(TestChoiceProvider))]
    [Option("option", "option")] string option)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent(option));
    }

    //For user commands
    [ContextMenu(ApplicationCommandType.UserContextMenu, "User Menu")]
    public async Task UserMenu(ContextMenuContext ctx) { }

    //For message commands
    [ContextMenu(ApplicationCommandType.MessageContextMenu, "Message Menu")]
    public async Task MessageMenu(ContextMenuContext ctx) { }
}
