namespace MMBot.Blazor.Client.Helpers;

public static class HttpClientHelpers
{
    public static async Task<IEnumerable<T>> GetAllEntities<T>(this HttpClient httpClient, ulong guildId, string typeName)
        => await httpClient.GetFromJsonAsync<IEnumerable<T>>($"api/{typeName}/getAll?guildId={guildId}");

    public static async Task<T> GetEntity<T>(this HttpClient httpClient, ulong id, string typeName)
        => await httpClient.GetFromJsonAsync<T>($"api/{typeName}?id={id}");

    public static async Task<T> CreateEntity<T>(this HttpClient httpClient, T entity, string typeName)
    {
        var resp = await httpClient.PostAsJsonAsync($"api/{typeName}", entity);

        return resp.IsSuccessStatusCode ? await resp.Content.ReadFromJsonAsync<T>() : default;
    }

    public static async Task<T> UpdateEntity<T>(this HttpClient httpClient, T entity, string typeName)
    {
        var resp = await httpClient.PutAsJsonAsync($"api/{typeName}", entity);

        return resp.IsSuccessStatusCode ? await resp.Content.ReadFromJsonAsync<T>() : default;
    }

    public static async Task DeleteEntity<T>(this HttpClient httpClient, ulong id, string typeName)
        => await httpClient.DeleteAsync($"api/{typeName}?id={id}");
}
