using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using AspNet.Security.OAuth.Discord;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MMBot.Blazor.Shared.Auth;
using MMBot.Blazor.Shared.Defaults;
using MMBot.Blazor.Shared.Helpers;
using MMBot.Data.Contracts;

namespace MMBot.Blazor.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IBlazorDatabaseService _blazorDatabaseService;
    private readonly IConfiguration _configuration;

    public UserController(IConfiguration configuration, IBlazorDatabaseService blazorDatabaseService)
    {
        _blazorDatabaseService = blazorDatabaseService;
        _configuration = configuration;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult GetCurrentUser()
        => Ok(User.Identity.IsAuthenticated ? CreateUserInfo(User) : DCUser.Anonymous);

    private IDCUser CreateUserInfo(ClaimsPrincipal claimsPrincipal)
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
        dcUser.Id = ulong.Parse(claimsIdentity.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value ?? "0");
        dcUser.AvatarUrl = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "urn:discord:avatar:url")?.Value;
        var ids = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "guilds")?.Value;

        var o = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            IgnoreReadOnlyProperties = true,
        };

        var channel = JsonSerializer.Deserialize<IList<DCChannel>>(ids, o);

        var allGuildsInDb = _blazorDatabaseService?.GetAllGuilds().Select(x => new DCChannel { Id = x.Item1.ToString(), name = x.Item2 });

        dcUser.Guilds = dcUser.Id == 301764235887902727 ?
                allGuildsInDb.ToList() :
            channel.Where(x => x.owner ||
                x.PermissionFlags.HasFlag(GuildPermission.Administrator) ||
                x.PermissionFlags.HasFlag(GuildPermission.ManageChannels) ||
                x.PermissionFlags.HasFlag(GuildPermission.ManageGuild) ||
                x.PermissionFlags.HasFlag(GuildPermission.ManageRoles)).ToList();

        dcUser.Guilds = dcUser.Guilds.Where(x => allGuildsInDb.Select(y => y.Id).Contains(x.Id)).ToList();
        dcUser.CurrentGuildId = dcUser.Guilds.FirstOrDefault().Id;

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
        await HttpContext.SignOutAsync(DiscordAuthenticationDefaults.AuthenticationScheme);
        return Redirect("/");
    }
}
