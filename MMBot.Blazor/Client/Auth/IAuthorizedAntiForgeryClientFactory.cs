namespace MMBot.Blazor.Client.Auth;

public interface IAuthorizedAntiForgeryClientFactory
{
    Task<HttpClient> CreateClient(string clientName = "authorizedClient");
}
