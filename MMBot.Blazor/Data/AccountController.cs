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
        public AccountController()
        {

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
    }
}
