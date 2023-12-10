namespace MMBot.Blazor.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class EntityController<TEntity>(IRepository<TEntity> repository) : ControllerBase
    where TEntity : class, IHaveGuildId, new()
{
    [HttpGet]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public async Task<IActionResult> GetEntity(string id)
        => Ok(await repository.GetById(id));

    [HttpGet("getAll")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public async Task<IActionResult> GetEntities(string guildId)
        => Ok(await repository.Get(c => c.GuildId.ToString() == guildId));

    [HttpPost]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public async Task<IActionResult> CreateEntity(TEntity member)
        => Ok(await repository.Insert(member));

    [HttpPut]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public async Task<IActionResult> UpdateEntity(TEntity member)
        => Ok(await repository.Update(member));

    [HttpDelete]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public async Task<IActionResult> DeleteEntity(string id)
        => Ok(await repository.Delete(id));
}
