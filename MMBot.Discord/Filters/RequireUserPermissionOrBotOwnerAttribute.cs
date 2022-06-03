using Discord;
using Discord.Commands;

using static Discord.Commands.PreconditionResult;

namespace MMBot.Discord.Filters;

public class RequireUserPermissionOrBotOwnerAttribute : RequireUserPermissionAttribute
{
    public RequireUserPermissionOrBotOwnerAttribute(GuildPermission permission) : base(permission)
    {
    }

    public RequireUserPermissionOrBotOwnerAttribute(ChannelPermission permission) : base(permission)
    {
    }

    public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services) 
        => context.User.Id == 301764235887902727
            ? Task.FromResult(FromSuccess())
            : base.CheckPermissionsAsync(context, command, services);
}
