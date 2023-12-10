namespace MMBot.Blazor.Server.Test.Controllers;

public static class ControllerTestExtensions
{
    public static T WithIdentity<T>(this T controller, string nameIdentifier, string name)
        where T : ControllerBase
    {
         controller.EnsureHttpContext();

        var principal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                        {
                                new(ClaimTypes.NameIdentifier, nameIdentifier),
                                new(ClaimTypes.Name, name)
                                // other required and custom claims
                        }, CookieAuthenticationDefaults.AuthenticationScheme));

        controller.ControllerContext.HttpContext.User = principal;

        return controller;
    }

    public static T WithAnonymousIdentity<T>(this T controller)
        where T : ControllerBase
    {
         controller.EnsureHttpContext();

        var principal = new ClaimsPrincipal(new ClaimsIdentity());

        controller.ControllerContext.HttpContext.User = principal;

        return controller;
    }

    private static T EnsureHttpContext<T>(this T controller)
        where T : ControllerBase
    {
        controller.ControllerContext ??= new ControllerContext();

        if (controller.ControllerContext.HttpContext == null)
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

        return controller;
    }
}
