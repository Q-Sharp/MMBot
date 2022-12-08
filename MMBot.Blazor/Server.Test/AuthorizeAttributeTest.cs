namespace MMBot.Blazor.Server.Test;

public class AuthorizeAttributeTest
{
    private static IEnumerable<Type> GetChildTypes<TEntitiy>()
        where TEntitiy : class
        => typeof(EntityController<>).Assembly.GetTypes()
                .Where(t =>  !t.IsAbstract);

    [Fact]
    public void ApiAndMVCControllersShouldHaveAuthorizeAttribute()
    {
        var authA = GetChildTypes<object>()
            .Where(t => t.Name != "UserController") // User is anonymous before using api auth!!
            .Select(t => Attribute.GetCustomAttribute(t, typeof(AuthorizeAttribute), true) as AuthorizeAttribute);

        Assert.All(authA, Assert.NotNull);
    }
}
