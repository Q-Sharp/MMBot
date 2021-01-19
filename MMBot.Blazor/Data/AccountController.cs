using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;

namespace MMBot.Blazor.Data
{
    [Route("[controller]/[action]")] // Microsoft.AspNetCore.Mvc.Route
    public class AccountController : ControllerBase
    {
        private readonly IDataProtectionProvider _provider;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IAuthenticationService _authenticationService;

        public AccountController(IDataProtectionProvider provider, IHttpClientFactory clientFactory, IAuthenticationService authenticationService)
        {
            _provider = provider;
            _clientFactory = clientFactory;
            _authenticationService = authenticationService;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = "/") 
            => Challenge(new AuthenticationProperties { RedirectUri = returnUrl }, "Discord");

        [HttpGet]
        public async Task<IActionResult> Logout(string returnUrl = "/")
        {
            //This removes the cookie assigned to the user login.
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return LocalRedirect(returnUrl);
        }

        [HttpGet]
        public async Task<IActionResult> GetGuildIds(string returnUrl = "/")
        {
            var client = _clientFactory.CreateClient();
            //var token1 = await _authenticationService.GetTokenAsync(HttpContext, "Discord");
            //var token2 = await _authenticationService.GetTokenAsync(HttpContext, "Bearer");


            //var discord = _discordSocketClient.LoginAsync(Discord.TokenType.Bearer, )
            //authState.User.Identity.

            var rq = new HttpRequestMessage(HttpMethod.Get, "https://discordapp.com/api/users/@me/guilds");
            //rq.Headers.Add("Authentication", token);
            var r = await client.SendAsync(rq);
            return LocalRedirect(returnUrl);
            //return await r.Content.ReadAsStringAsync();
        }
    }
}
