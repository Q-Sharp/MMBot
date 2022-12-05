namespace MMBot.Blazor.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class ApiControllerBase<TController, TEntity> : ControllerBase
    where TEntity : class
{
    protected ILogger<TController> _logger { get; }
    protected IRepository<TEntity> _repository { get; }

    public ApiControllerBase(ILogger<TController> logger, IRepository<TEntity> repository)
    {
        _logger = logger;
        _repository = repository;
    }
}
