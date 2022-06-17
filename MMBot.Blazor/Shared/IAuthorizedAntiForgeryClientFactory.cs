namespace MMBot.Blazor.Shared;

public interface IAuthorizedAntiForgeryClientFactory
{
    Task<HttpClient> CreateClient(string clientName = "authorizedClient");
}
