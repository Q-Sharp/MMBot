using MMBot.Data.Contracts.Entities;

namespace MMBot.Blazor.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class HelpController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetHelp([FromServices] IRepository<Member> repo)
    {
        return await Task.Run(() => Ok(repo.GetById(0))); 
    }
}
