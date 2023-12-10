namespace MMBot.Blazor.Server.Auth;

public class MMBotTicketStore(IMemoryCache cache) : ITicketStore
{
    private const string _keyPrefix = "__AUTHTICKETSTORE";

    public async Task<string> StoreAsync(AuthenticationTicket ticket)
    {
        var key = $"{_keyPrefix}{Guid.NewGuid()}";
        await RenewAsync(key, ticket);
        return key;
    }

    public Task RenewAsync(string key, AuthenticationTicket ticket)
    {
        // https://github.com/aspnet/Caching/issues/221
        // Set to "NeverRemove" to prevent undesired evictions from gen2 GC
        var options = new MemoryCacheEntryOptions
        {
            Priority = CacheItemPriority.NeverRemove
        };
        var expiresUtc = ticket.Properties.ExpiresUtc;

        if (expiresUtc.HasValue)
             options.SetAbsoluteExpiration(expiresUtc.Value);

         options.SetSlidingExpiration(TimeSpan.FromMinutes(60));

         cache.Set(key, ticket, options);

        return Task.FromResult(0);
    }

    public Task<AuthenticationTicket> RetrieveAsync(string key)
    {
         cache.TryGetValue(key, out AuthenticationTicket ticket);
        return Task.FromResult(ticket);
    }

    public Task RemoveAsync(string key)
    {
        cache.Remove(key);
        return Task.FromResult(0);
    }
}
