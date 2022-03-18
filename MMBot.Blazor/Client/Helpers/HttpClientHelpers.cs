using System.Net.Http.Json;
using MMBot.Data.Contracts.Entities;

namespace MMBot.Blazor.Client.Helpers;

public static class HttpClientHelpers
{
    public async static Task<IEnumerable<T>> GetAllEntities<T>(this HttpClient httpClient, ulong guildId) 
        => await httpClient.GetFromJsonAsync<IEnumerable<T>>($"api/{typeof(T).Name}/getAll/guildId={guildId}");

    public async static Task<string> GetAllEntities(this HttpClient httpClient, ulong guildId)
        => await httpClient.GetStringAsync($"api/{typeof(Clan).Name}/getAll?id={guildId}");

    public async static Task<T> GetEntity<T>(this HttpClient httpClient, ulong id)
        => await httpClient.GetFromJsonAsync<T>($"api/{typeof(T).Name}/id={id}");

    public async static Task<T> CreateEntity<T>(this HttpClient httpClient, T entity)
    {
        var resp = await httpClient.PostAsJsonAsync($"api/{typeof(T).Name}", entity);

        if(resp.IsSuccessStatusCode)
            return await resp.Content.ReadFromJsonAsync<T>();

        return default;
    }

    public async static Task<T> UpdateEntity<T>(this HttpClient httpClient, T entity)
    {
        var resp = await httpClient.PutAsJsonAsync($"api/{typeof(T).Name}", entity);

        if (resp.IsSuccessStatusCode)
            return await resp.Content.ReadFromJsonAsync<T>();

        return default;
    }

    public async static Task DeleteEntity<T>(this HttpClient httpClient, ulong id)
        => await httpClient.DeleteAsync($"api/{typeof(T).Name}/id={id}");
}
