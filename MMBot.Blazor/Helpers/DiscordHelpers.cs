using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OAuth;
using Newtonsoft.Json.Linq;

namespace MMBot.Blazor.Helpers
{
    public class DiscordHelpers
    {
        public async static Task<Claim> GetGuildClaims(OAuthCreatingTicketContext context)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://discordapp.com/api/users/@me/guilds");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

            var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, CancellationToken.None);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("failed to get guilds");
            }

            var payload = JArray.Parse(await response.Content.ReadAsStringAsync());
            //var claim = new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/guilds", payload.ToString(), ClaimValueTypes.String);
            var claim = new Claim("guilds", payload.ToString(), ClaimValueTypes.String);
            return claim;
        }
    }
}
