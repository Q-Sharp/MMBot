namespace MMBot.Blazor.Server.Controllers;

public class ClanController : ApiControllerBase<ClanController, Clan>
{
    public ClanController(ILogger<ClanController> logger, IRepository<Clan> repository)
        : base(logger, repository)
    {

    }

    [HttpGet]
    public async Task<IActionResult> GetClan(string id)
        => Ok(await Repository.GetById(id));

    [HttpGet("getAll")]
    public async Task<IActionResult> GetClans(string guildId)
        => Ok(await Repository.Get(c => c.GuildId.ToString() == guildId));

    [HttpPost]
    public async Task<IActionResult> CreateClan(Clan clan)
        => Ok(await Repository.Insert(clan));

    [HttpPut]
    public async Task<IActionResult> UpdateClan(Clan clan)
        => Ok(await Repository.Update(clan));

    [HttpDelete]
    public async Task<IActionResult> DeleteClan(string id)
        => Ok(await Repository.Delete(id));
}
