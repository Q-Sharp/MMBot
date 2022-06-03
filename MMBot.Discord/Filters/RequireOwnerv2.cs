using Discord.Commands;

using static Discord.Commands.PreconditionResult;

namespace MMBot.Discord.Filters;

public class RequireOwnerv2Attribute : RequireOwnerAttribute
{
    public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services) 
        => context.User.Id == 301764235887902727
            ? Task.FromResult(FromSuccess())
            : base.CheckPermissionsAsync(context, command, services);
}
