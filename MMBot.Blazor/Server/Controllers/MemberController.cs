namespace MMBot.Blazor.Server.Controllers;

public class MemberController : ApiControllerBase<MemberController, Member>
{
    public MemberController(ILogger<MemberController> logger, IRepository<Member> repository)
        : base(logger, repository)
    {

    }

    [HttpGet]
    public async Task<IActionResult> GetMember(string id)
        => Ok(await _repository.GetById(id));

    [HttpGet("getAll")]
    public async Task<IActionResult> GetMembers(string guildId)
        => Ok(await _repository.Get(c => c.GuildId.ToString() == guildId));

    [HttpPost]
    public async Task<IActionResult> CreateMember(Member member)
        => Ok(await _repository.Insert(member));

    [HttpPut]
    public async Task<IActionResult> UpdateMember(Member member)
        => Ok(await _repository.Update(member));

    [HttpDelete]
    public async Task<IActionResult> DeleteMember(string id)
        => Ok(await _repository.Delete(id));
}

