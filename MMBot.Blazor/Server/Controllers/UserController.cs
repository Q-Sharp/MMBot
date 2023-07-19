namespace MMBot.Blazor.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IBlazorDatabaseService _blazorDatabaseService;

    public UserController(IBlazorDatabaseService blazorDatabaseService)
    {
        _blazorDatabaseService = blazorDatabaseService;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult GetCurrentUser() => Ok(User.Identity.IsAuthenticated ? CreateUserInfo(User) : DCUser.Anonymous);

    private DCUser CreateUserInfo(ClaimsPrincipal claimsPrincipal)
    {
        if (!claimsPrincipal.Identity.IsAuthenticated)
            return DCUser.Anonymous;

        var dcUser = new DCUser
        {
            IsAuthenticated = true
        };

        var claimsIdentity = claimsPrincipal?.Identity as ClaimsIdentity;

        if (claimsIdentity is not null)
        {
            dcUser.NameClaimType = claimsIdentity.NameClaimType;
            dcUser.RoleClaimType = claimsIdentity.RoleClaimType;
        }
        else
        {
            dcUser.NameClaimType = ClaimTypes.Name;
            dcUser.RoleClaimType = ClaimTypes.Role;
        }

        if (claimsPrincipal.Claims.Any())
        {
            var allClaims = claimsPrincipal.Claims
                .Select(x => new ClaimValue(x.Type, x.Value))
                .ToList();

            dcUser.Claims = allClaims;
        }

        dcUser.Name = claimsIdentity.Name;
        dcUser.Id = ulong.Parse(claimsIdentity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "0");
        var avaHash = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "urn:discord:avatar:hash")?.Value;
        dcUser.AvatarUrl = @$"https://cdn.discordapp.com/avatars/{dcUser.Id}/{avaHash}.png";

        var ids = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "guilds")?.Value;

        if(ids is not null)
        {
            var o = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                IgnoreReadOnlyProperties = true,
            };

            var channel = JsonSerializer.Deserialize<IList<DCChannel>>(ids, o);
            var allGuildsInDb = _blazorDatabaseService?.GetAllGuilds().Select(x => new DCChannel { Id = x.Id.ToString(), Name = x.Name });

            dcUser.Guilds = dcUser.IsOwner() ?
                    allGuildsInDb.ToList() :
                channel.Where(x => x.Owner ||
                    x.PermissionFlags.HasFlag(GuildPermission.Administrator) ||
                    x.PermissionFlags.HasFlag(GuildPermission.ManageChannels) ||
                    x.PermissionFlags.HasFlag(GuildPermission.ManageGuild) ||
                    x.PermissionFlags.HasFlag(GuildPermission.ManageRoles)).ToList();

            dcUser.Guilds = dcUser.Guilds.Where(x => allGuildsInDb.Select(y => y.Id).Contains(x.Id)).ToList();
        }

        if(dcUser.Guilds is null || dcUser.Guilds.Count is 0)
        {
            dcUser.Guilds = new DCChannel[]
            {
                new DCChannel
                {
                    Id = "0",
                    Name = "No Guilds to manage",
                }
            };
        }

        return dcUser;
    }

    [HttpGet(ApiAuthDefaults.LogIn)]
    public IActionResult Login(string returnUrl = null) => Challenge(new AuthenticationProperties
    {
        RedirectUri = !string.IsNullOrEmpty(returnUrl) ? returnUrl : "/",
    });

    [HttpGet(ApiAuthDefaults.LogOut)]
    public async Task<IActionResult> LogOut()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Redirect("/");
    }
}
