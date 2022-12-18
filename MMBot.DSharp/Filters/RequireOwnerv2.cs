namespace MMBot.DSharp.Filters;

public class RequireOwnerv2Attribute : SlashCheckBaseAttribute
{
    public override Task<bool> ExecuteChecksAsync(InteractionContext ctx)
    {
        ctx.User.IsOwner()
    }
}
