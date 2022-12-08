namespace MMBot.Blazor.Server.Controllers;

public class EntityController<TEntity> : MMControllerBase
    where TEntity : class, IHaveGuildId, new()
{
    private readonly IRepository<TEntity> _repository;

    public EntityController(IRepository<TEntity> repository)
        => _repository = repository;

    [HttpGet]
    public async Task<IActionResult> GetEntity(string id)
        => Ok(await _repository.GetById(id));

    [HttpGet("getAll")]
    public async Task<IActionResult> GetEntities(string guildId)
        => Ok(await _repository.Get(c => c.GuildId.ToString() == guildId));

    [HttpPost]
    public async Task<IActionResult> CreateEntity(TEntity member)
        => Ok(await _repository.Insert(member));

    [HttpPut]
    public async Task<IActionResult> UpdateEntity(TEntity member)
        => Ok(await _repository.Update(member));

    [HttpDelete]
    public async Task<IActionResult> DeleteEntity(string id)
        => Ok(await _repository.Delete(id));
}
