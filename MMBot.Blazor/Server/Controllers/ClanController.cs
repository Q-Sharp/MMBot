namespace MMBot.Blazor.Server.Controllers;

public class ClanController : ApiControllerBase<ClanController, Clan>
{
    public ClanController(ILogger<ClanController> logger, IRepository<Clan> repository)
        : base(logger, repository)
    {

    }

    [HttpGet("id={id}")]
    public async Task<IActionResult> GetClan(ulong id)
        => Ok(await Repository.GetById(id));

    [HttpGet("getAll")]
    public async Task<IActionResult> GetClans(ulong guildId)
        => Ok(await Repository.Get(c => c.GuildId == guildId));

    [HttpPost]
    public async Task<IActionResult> CreateClan(Clan clan)
        => Ok(await Repository.Insert(clan));

    [HttpPut]
    public async Task<IActionResult> UpdateClan(Clan clan)
        => Ok(await Repository.Update(clan));

    [HttpDelete]
    public async Task<IActionResult> DeleteClan(ulong id)
        => Ok(await Repository.Delete(id));
}
