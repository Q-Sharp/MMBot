namespace MMBot.Blazor.Server.Test.Controllers;

public class UserControllerTests
{
    [Fact]
    public void LoginTest()
    {
        var dbs = A.Fake<IBlazorDatabaseService>();
        var userController = new UserController(dbs);

        var result = userController.Login("test");

        var cr = Assert.IsAssignableFrom<ChallengeResult>(result);
        Assert.Equal("test", cr.Properties.RedirectUri);
    }

    [Fact]
    public void GetCurrentUserWithAnonymousIdentityTest()
    {
        var dbs = A.Fake<IBlazorDatabaseService>();
        var userController = new UserController(dbs).WithAnonymousIdentity();
        var result = userController.GetCurrentUser();

        var or = Assert.IsAssignableFrom<OkObjectResult>(result);
        var ui = Assert.IsAssignableFrom<DCUser>(or.Value);

        Assert.False(ui.IsAuthenticated);
    }

    [Fact]
    public void GetCurrentUserWithIdentityTest()
    {
        //var config = A.Fake<IConfiguration>();
        //var dbs = A.Fake<IBlazorDatabaseService>();
        //var userController = new UserController(config, dbs).WithIdentity("test", "tester");
        //var result = userController.GetCurrentUser();

        //var or = Assert.IsAssignableFrom<OkObjectResult>(result);
        //var ui = Assert.IsAssignableFrom<IDCUser>(or.Value);

        //Assert.True(ui.IsAuthenticated);
        //Assert.Contains("tester", ui.Claims.Select(x => x.Value));
    }
}
