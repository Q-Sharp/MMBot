using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MMBot.Blazor.Shared;

namespace MMBot.Blazor.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[ValidateAntiForgeryToken]
[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class ApiControllerBase<TController, TEntity> : ControllerBase
    where TEntity : class
{
    protected ILogger<TController> Logger { get; }
    protected IRepository<TEntity> Repository { get; }

    public ApiControllerBase(ILogger<TController> logger, IRepository<TEntity> repository) 
    {
        Logger = logger;
        Repository = repository;
    } 
}
