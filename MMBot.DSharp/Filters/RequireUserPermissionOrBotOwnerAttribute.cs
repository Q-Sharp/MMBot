//using static Discord.Commands.PreconditionResult;

//namespace MMBot.DSharp.Filters;

//public class RequireUserPermissionOrBotOwnerAttribute : RequireUserPermissionAttribute
//{
//    public RequireUserPermissionOrBotOwnerAttribute(GuildPermission permission) : base(permission)
//    {
//    }

//    public RequireUserPermissionOrBotOwnerAttribute(ChannelPermission permission) : base(permission)
//    {
//    }

//    public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
//        => context.User.IsOwner()
//            ? Task.FromResult(FromSuccess())
//            : base.CheckPermissionsAsync(context, command, services);
//}
