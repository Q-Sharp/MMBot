namespace MMBot.Blazor.Client.Helpers;

public static class HttpClientHelpers
{
    public async static Task<IEnumerable<T>> GetAllEntities<T>(this HttpClient httpClient, ulong guildId, string typeName) 
        => await httpClient.GetFromJsonAsync<IEnumerable<T>>($"api/{typeName}/getAll?guildId={guildId}");

    public async static Task<T> GetEntity<T>(this HttpClient httpClient, ulong id, string typeName)
        => await httpClient.GetFromJsonAsync<T>($"api/{typeName}?id={id}");

    public async static Task<T> CreateEntity<T>(this HttpClient httpClient, T entity, string typeName)
    {
        var resp = await httpClient.PostAsJsonAsync($"api/{typeName}", entity);

        if(resp.IsSuccessStatusCode)
            return await resp.Content.ReadFromJsonAsync<T>();

        return default;
    }

    public async static Task<T> UpdateEntity<T>(this HttpClient httpClient, T entity, string typeName)
    {
        var resp = await httpClient.PutAsJsonAsync($"api/{typeName}", entity);

        if (resp.IsSuccessStatusCode)
            return await resp.Content.ReadFromJsonAsync<T>();

        return default;
    }

    public async static Task DeleteEntity<T>(this HttpClient httpClient, ulong id, string typeName)
        => await httpClient.DeleteAsync($"api/{typeName}?id={id}");
}
