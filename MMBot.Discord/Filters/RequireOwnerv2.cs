using Discord.Commands;
using MMBot.Discord.Helpers;
using static Discord.Commands.PreconditionResult;

namespace MMBot.Discord.Filters;

public class RequireOwnerv2Attribute : RequireOwnerAttribute
{
    public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services) 
        => context.User.IsOwner()
            ? Task.FromResult(FromSuccess())
            : base.CheckPermissionsAsync(context, command, services);
}
