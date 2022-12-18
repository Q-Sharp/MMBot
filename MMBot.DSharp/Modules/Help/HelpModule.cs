//using Discord;
//using Discord.Commands;
//using MMBot.Data.Contracts;
//using MMBot.DSharp.Modules;
//using MMBot.DSharp.Modules.Interfaces;
//using MMBot.DSharp.Services.Interfaces;

//namespace MMBot.DSharp.Modules.Help;

//[Name("Help")]
//public class HelpModule : MMBotModule, IHelpModule
//{
//    private readonly CommandService _service;

//    public HelpModule(CommandService service, IDatabaseService databaseService, IGuildSettingsService guildSettings, ICommandHandler commandHandler)
//        : base(databaseService, guildSettings, commandHandler) => _service = service;

//    [Command("help")]
//    public async Task<RuntimeResult> HelpAsync()
//    {
//        var settings = await _guildSettingsService.GetGuildSettingsAsync(Context.Guild.Id);

//        var builder = new EmbedBuilder()
//        {
//            Color = new Color(114, 137, 218),
//            Description = "These are the commands you can use:",
//            Footer = new EmbedFooterBuilder()
//            {
//                Text = "To get more information for each command add the command name behind the help command!"
//            }
//        };

//        foreach (var module in _service.Modules)
//        {
//            string description = null;
//            foreach (var cmd in module.Commands.Where(x => x.Preconditions.All(attribute => attribute.GetType() != typeof(RequireOwnerAttribute))).Distinct().ToArray())
//            {
//                var result = await cmd.CheckPreconditionsAsync(Context);
//                if (!result.IsSuccess)
//                    continue;

//                var args = string.Join(" ", cmd.Parameters?.Select(x => $"[{x.Name}]").ToArray() ?? Array.Empty<string>());

//                if (string.Equals(cmd.Name, module.Group, StringComparison.InvariantCultureIgnoreCase))
//                    description += $"{settings.Prefix}{module.Group} {args}{Environment.NewLine}";
//                else if (string.IsNullOrWhiteSpace(module.Group))
//                    description += $"{settings.Prefix}{cmd.Name} {args}{Environment.NewLine}";
//                else
//                    description += $"{settings.Prefix}{module.Group} {cmd.Name} {args}{Environment.NewLine}";
//            }

//            if (!string.IsNullOrWhiteSpace(description))
//                builder.AddField(x =>
//                {
//                    x.Name = module.Name;
//                    x.Value = description;
//                    x.IsInline = false;
//                });
//        }

//        await ReplyAsync("", false, builder.Build());
//        return FromSuccess();
//    }

//    [Command("help")]
//    public async Task<RuntimeResult> HelpAsync([Remainder] string command)
//    {
//        var result = _service.Search(Context, command);

//        if (!result.IsSuccess)
//            return FromErrorObjectNotFound("command", command);

//        var builder = new EmbedBuilder()
//        {
//            Color = new Color(114, 137, 218),
//            Description = $"Here are some commands like **{command}**"
//        };

//        foreach (var match in result.Commands)
//        {
//            var cmd = match.Command;

//            builder.AddField(x =>
//            {
//                x.Name = string.Join(", ", cmd.Aliases);
//                x.Value = $"Parameters: {string.Join(", ", cmd.Parameters.Select(p => p.Name))}\n" +
//                          $"Summary: {cmd.Summary}";
//                x.IsInline = false;
//            });
//        }

//        await ReplyAsync("", false, builder.Build());
//        return FromSuccess();
//    }
//}
