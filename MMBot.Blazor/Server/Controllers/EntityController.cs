namespace MMBot.Blazor.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class EntityController<TEntity> : ControllerBase
    where TEntity : class, IHaveGuildId, new()
{
    private readonly IRepository<TEntity> _repository;

    public EntityController(IRepository<TEntity> repository)
        => _repository = repository;

    [HttpGet]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public async Task<IActionResult> GetEntity(string id)
        => Ok(await _repository.GetById(id));

    [HttpGet("getAll")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public async Task<IActionResult> GetEntities(string guildId)
        => Ok(await _repository.Get(c => c.GuildId.ToString() == guildId));

    [HttpPost]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public async Task<IActionResult> CreateEntity(TEntity member)
        => Ok(await _repository.Insert(member));

    [HttpPut]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public async Task<IActionResult> UpdateEntity(TEntity member)
        => Ok(await _repository.Update(member));

    [HttpDelete]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public async Task<IActionResult> DeleteEntity(string id)
        => Ok(await _repository.Delete(id));
}
