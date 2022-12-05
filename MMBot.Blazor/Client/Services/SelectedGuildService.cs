namespace MMBot.Blazor.Client.Services;

public class SelectedGuildService : ISelectedGuildService
{
    private readonly MMBotAuthenticationStateProvider _authenticationStateProvider;
    private readonly ISessionStorageService _sessionStorage;

    public SelectedGuildService(MMBotAuthenticationStateProvider authenticationStateProvider, ISessionStorageService sessionStorage)
        => (_authenticationStateProvider, _sessionStorage) = (authenticationStateProvider, sessionStorage);

    public async Task<IEnumerable<DCChannel>> GetGuilds()
    {
        var loggedUser = await _authenticationStateProvider.GetCurrentUser();

        if (loggedUser.IsAuthenticated)
        {
            var stored = (await _sessionStorage.GetItemAsync<ulong>(SessionStoreDefaults.GuildId)).ToString();
            (loggedUser.Guilds.FirstOrDefault(g => g.Id == stored) ?? loggedUser.Guilds.FirstOrDefault()).IsSelected = true;

            return loggedUser.Guilds;
        }

        return Enumerable.Empty<DCChannel>();
    }

    public async Task<ulong> GetSelectedGuildId() 
        => await _sessionStorage.GetItemAsync<ulong>(SessionStoreDefaults.GuildId);

    public async Task SetSelectedGuild(string id)
        => await _sessionStorage.SetItemAsync(SessionStoreDefaults.GuildId, ulong.Parse(id));
}
