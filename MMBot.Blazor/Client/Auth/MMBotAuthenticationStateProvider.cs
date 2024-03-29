﻿namespace MMBot.Blazor.Client.Auth;

public class MMBotAuthenticationStateProvider : AuthenticationStateProvider
{
    private static readonly TimeSpan _userCacheRefreshInterval = TimeSpan.FromSeconds(60);

    private readonly NavigationManager _navigation;
    private readonly HttpClient _client;
    private readonly ILogger<MMBotAuthenticationStateProvider> _logger;

    private DateTimeOffset _userLastCheck = DateTimeOffset.FromUnixTimeSeconds(0);
    private ClaimsPrincipal _cachedUser = new(new ClaimsIdentity());

    public MMBotAuthenticationStateProvider(NavigationManager navigation, HttpClient client, ILogger<MMBotAuthenticationStateProvider> logger)
    {
        _navigation = navigation;
        _client = client;
        _logger = logger;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        => new AuthenticationState(await GetUser(true));

    public async Task<DCUser> GetCurrentUser()
    {
        DCUser user = null;

        try
        {
            user = await _client.GetFromJsonAsync<DCUser>(ApiAuthDefaults.UserPath);
        }
        catch (Exception exc)
        {
            _logger.LogWarning(exc, "Fetching user failed.");
        }

        return user;
    }

    public void SignIn(string customReturnUrl = null)
    {
        var returnUrl = customReturnUrl != null ? _navigation.ToAbsoluteUri(customReturnUrl).ToString() : null;
        var encodedReturnUrl = Uri.EscapeDataString(returnUrl ?? _navigation.Uri);
        var logInUrl = _navigation.ToAbsoluteUri($"{ApiAuthDefaults.LogInPath}?returnUrl={encodedReturnUrl}");
        _navigation.NavigateTo(logInUrl.ToString(), true);
    }

    private async ValueTask<ClaimsPrincipal> GetUser(bool useCache = false)
    {
        var now = DateTimeOffset.Now;
        if (useCache && now < _userLastCheck + _userCacheRefreshInterval)
        {
            _logger.LogDebug("Taking user from cache");
            return _cachedUser;
        }

        _logger.LogDebug("Fetching user");
        _cachedUser = await FetchUser();
        _userLastCheck = now;

        return _cachedUser;
    }

    private async Task<ClaimsPrincipal> FetchUser()
    {
        DCUser user = null;

        try
        {
            user = await _client.GetFromJsonAsync<DCUser>(ApiAuthDefaults.UserPath);
        }
        catch (Exception exc)
        {
            _logger.LogWarning(exc, "Fetching user failed.");
        }

        if (user == null || !user.IsAuthenticated)
            return new ClaimsPrincipal(new ClaimsIdentity());

        var identity = new ClaimsIdentity(
                nameof(MMBotAuthenticationStateProvider),
                user.NameClaimType,
                user.RoleClaimType);

        if (user.Claims != null)
            identity.AddClaims(user.Claims.Select(x => new Claim(x.Type, x.Value)));

        return new ClaimsPrincipal(identity);
    }
}
