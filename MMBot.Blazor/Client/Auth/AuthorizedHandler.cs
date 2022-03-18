using System.Net;

namespace MMBot.Blazor.Client.Auth;

public class AuthorizedHandler : DelegatingHandler
{
    private readonly MMBotAuthenticationStateProvider _authenticationStateProvider;

    public AuthorizedHandler(MMBotAuthenticationStateProvider authenticationStateProvider)
        => _authenticationStateProvider = authenticationStateProvider;

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        HttpResponseMessage responseMessage;

        if (!authState.User.Identity.IsAuthenticated)
            // if user is not authenticated, immediately set response status to 401 Unauthorized
            responseMessage = new HttpResponseMessage(HttpStatusCode.Unauthorized);
        else
            responseMessage = await base.SendAsync(request, cancellationToken);

        if (responseMessage.StatusCode == HttpStatusCode.Unauthorized)
            // if server returned 401 Unauthorized, redirect to login page
            _authenticationStateProvider.SignIn();

        return responseMessage;
    }
}
