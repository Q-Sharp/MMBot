namespace MMBot.Blazor.Shared.Defaults;

public static class ApiAuthDefaults
{
    public const string UserPath = "api/User";
    public const string LogIn = "Login";
    public const string LogOut = "Logout";

    public const string LogInPath = $"{UserPath}/{LogIn}";
    public const string LogOutPath = $"{UserPath}/{LogOut}";
}
