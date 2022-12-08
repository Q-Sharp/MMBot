namespace MMBot.Blazor.Client.Services;

public class SelectedGuildService : ISelectedGuildService
{
    private readonly MMBotAuthenticationStateProvider _authenticationStateProvider;
    private readonly ISessionStorageService _sessionStorage;
    private readonly ILogger<SelectedGuildService> _logger;

    public SelectedGuildService(MMBotAuthenticationStateProvider authenticationStateProvider, ISessionStorageService sessionStorage, ILogger<SelectedGuildService> logger)
        => (_authenticationStateProvider, _sessionStorage, _logger) = (authenticationStateProvider, sessionStorage, logger);

    public event EventHandler<EventArgs> Changed;

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
    {
        try
        {
            if (ulong.TryParse(id, out var uid))
            {
                await _sessionStorage.SetItemAsync(SessionStoreDefaults.GuildId, uid);
                Changed?.Invoke(this, EventArgs.Empty);
            }
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "set selected guild problem");
        }
    }
}
