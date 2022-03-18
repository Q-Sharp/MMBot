
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using MMBot.Blazor.Server.Controllers;

using Xunit;

namespace MMBot.Blazor.Server.Test;

public class AuthorizeAttributeTest
{
    private static IEnumerable<Type> GetChildTypes<TController, TEntitiy>()
        where TEntitiy : class
        => typeof(ApiControllerBase<TController, TEntitiy>).Assembly.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(TController)) && !t.IsAbstract);

    [Fact]
    public void ApiAndMVCControllersShouldHaveAuthorizeAttribute()
    {
        var authA = GetChildTypes<ControllerBase, object>()
            .Where(t => t.Name != "UserController") // User is anonymous before using api auth!!
            .Select(t => Attribute.GetCustomAttribute(t, typeof(AuthorizeAttribute), true) as AuthorizeAttribute);

        Assert.All(authA, a => Assert.NotNull(a));
    }
}
