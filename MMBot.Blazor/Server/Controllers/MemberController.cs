namespace MMBot.Blazor.Server.Controllers;

public class MemberController : ApiControllerBase<MemberController, Member>
{
    public MemberController(ILogger<MemberController> logger, IRepository<Member> repository)
        : base(logger, repository)
    {

    }

    [HttpGet]
    public async Task<IActionResult> GetMember(string id)
        => Ok(await Repository.GetById(id));

    [HttpGet("getAll")]
    public async Task<IActionResult> GetMembers(string guildId)
        => Ok(await Repository.Get(c => c.GuildId.ToString() == guildId));

    [HttpPost]
    public async Task<IActionResult> CreateMember(Member member)
        => Ok(await Repository.Insert(member));

    [HttpPut]
    public async Task<IActionResult> UpdateMember(Member member)
        => Ok(await Repository.Update(member));

    [HttpDelete]
    public async Task<IActionResult> DeleteMember(string id)
        => Ok(await Repository.Delete(id));
}

