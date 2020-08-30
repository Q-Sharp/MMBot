using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace TTMMBot.Modules
{
    [Name("Help")]
    public class HelpModule : ModuleBase<SocketCommandContext>, IHelpModule
    {
        public GlobalSettings Gm { get; set; }


        private readonly CommandService _service;

        public HelpModule(CommandService service) => _service = service;

        [Command("help")]
        public async Task HelpAsync()
        {
            var builder = new EmbedBuilder()
            {
                Color = new Color(114, 137, 218),
                Description = "These are the commands you can use:",
                Footer = new EmbedFooterBuilder()
                {
                    Text = "To get more information for each command add the command name behind the help command!"
                }
            };

            foreach (var module in _service.Modules)
            {
                string description = null;
                foreach (var cmd in module.Commands.Where(x => x.Preconditions.All(attribute => attribute.GetType() != typeof(RequireOwnerAttribute))).Distinct().ToArray())
                {
                    var result = await cmd.CheckPreconditionsAsync(Context);
                    if (!result.IsSuccess)
                        continue;

                    var args = string.Join(" ", cmd.Parameters?.Select(x => $"[{x.Name}]").ToArray() ?? Array.Empty<string>());

                    if (string.Equals(cmd.Name, module.Group, StringComparison.InvariantCultureIgnoreCase))
                        description += $"{Gm.Prefix}{module.Group} {args}{Environment.NewLine}";
                    else if(string.IsNullOrWhiteSpace(module.Group))
                        description += $"{Gm.Prefix}{cmd.Name} {args}{Environment.NewLine}";
                    else
                        description += $"{Gm.Prefix}{module.Group} {cmd.Name} {args}{Environment.NewLine}";
                }

                if (!string.IsNullOrWhiteSpace(description))
                {
                    builder.AddField(x =>
                    {
                        x.Name = module.Name;
                        x.Value = description;
                        x.IsInline = false;
                    });
                }
            }

            await ReplyAsync("", false, builder.Build());
        }

        [Command("help")]
        public async Task HelpAsync([Remainder] string command)
        {
            var result = _service.Search(Context, command);

            if (!result.IsSuccess)
            {
                await ReplyAsync($"Sorry, I couldn't find a command like **{command}**.");
                return;
            }

            var builder = new EmbedBuilder()
            {
                Color = new Color(114, 137, 218),
                Description = $"Here are some commands like **{command}**"
            };

            foreach (var match in result.Commands)
            {
                var cmd = match.Command;

                builder.AddField(x =>
                {
                    x.Name = string.Join(", ", cmd.Aliases);
                    x.Value = $"Parameters: {string.Join(", ", cmd.Parameters.Select(p => p.Name))}\n" +
                              $"Summary: {cmd.Summary}";
                    x.IsInline = false;
                });
            }

            await ReplyAsync("", false, builder.Build());
        }
    }
}
