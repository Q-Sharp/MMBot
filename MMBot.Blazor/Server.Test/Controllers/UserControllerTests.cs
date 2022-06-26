using FakeItEasy;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MMBot.Blazor.Server.Controllers;
using MMBot.Blazor.Shared.Auth;
using MMBot.Data.Contracts;
using Xunit;

namespace MMBot.Blazor.Server.Test.Controllers;

public class UserControllerTests
{
    [Fact]
    public void LoginTest()
    {
        var config = A.Fake<IConfiguration>();
        var dbs = A.Fake<IBlazorDatabaseService>();
        var userController = new UserController(config, dbs);

        var result = userController.Login("test");

        var cr = Assert.IsAssignableFrom<ChallengeResult>(result);
        Assert.Equal("test", cr.Properties.RedirectUri);
    }

    [Fact]
    public void GetCurrentUserWithAnonymousIdentityTest()
    {
        var config = A.Fake<IConfiguration>();
        var dbs = A.Fake<IBlazorDatabaseService>();
        var userController = new UserController(config, dbs).WithAnonymousIdentity();
        var result = userController.GetCurrentUser();

        var or = Assert.IsAssignableFrom<OkObjectResult>(result);
        var ui = Assert.IsAssignableFrom<IDCUser>(or.Value);

        Assert.False(ui.IsAuthenticated);
    }

    [Fact]
    public void GetCurrentUserWithIdentityTest()
    {
        var config = A.Fake<IConfiguration>();
        var dbs = A.Fake<IBlazorDatabaseService>();
        var userController = new UserController(config, dbs).WithIdentity("test", "tester");
        var result = userController.GetCurrentUser();

        var or = Assert.IsAssignableFrom<OkObjectResult>(result);
        var ui = Assert.IsAssignableFrom<IDCUser>(or.Value);

        Assert.True(ui.IsAuthenticated);
        Assert.Contains("tester", ui.Claims.Select(x => x.Value));
    }
}
