using Microsoft.AspNetCore.Authentication.OAuth;

namespace MMBot.Blazor.Shared.Helpers;

public class DiscordHelpers
{
    public static async Task<Claim> GetGuildClaims(OAuthCreatingTicketContext context)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "https://discordapp.com/api/users/@me/guilds");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

        var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, CancellationToken.None);
        if (!response.IsSuccessStatusCode)
            throw new Exception("failed to get guilds");

        var payload = await response.Content.ReadAsStringAsync();
        var claim = new Claim("guilds", payload, ClaimValueTypes.String);
        return claim;
    }
}
