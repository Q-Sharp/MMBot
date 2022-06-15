using System.Net.Http.Json;

namespace MMBot.Blazor.Client.Helpers;

public static class HttpClientHelpers
{
    public async static Task<IEnumerable<T>> GetAllEntities<T>(this HttpClient httpClient, ulong guildId) 
        => await httpClient.GetFromJsonAsync<IEnumerable<T>>($"api/{nameof(T)}/getAll?guildId={guildId}");

    public async static Task<T> GetEntity<T>(this HttpClient httpClient, ulong id)
        => await httpClient.GetFromJsonAsync<T>($"api/{nameof(T)}?id={id}");

    public async static Task<T> CreateEntity<T>(this HttpClient httpClient, T entity)
    {
        var resp = await httpClient.PostAsJsonAsync($"api/{nameof(T)}", entity);

        if(resp.IsSuccessStatusCode)
            return await resp.Content.ReadFromJsonAsync<T>();

        return default;
    }

    public async static Task<T> UpdateEntity<T>(this HttpClient httpClient, T entity)
    {
        var resp = await httpClient.PutAsJsonAsync($"api/{nameof(T)}", entity);

        if (resp.IsSuccessStatusCode)
            return await resp.Content.ReadFromJsonAsync<T>();

        return default;
    }

    public async static Task DeleteEntity<T>(this HttpClient httpClient, ulong id)
        => await httpClient.DeleteAsync($"api/{nameof(T)}?id={id}");
}
