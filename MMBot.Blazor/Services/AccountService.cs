using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace MMBot.Blazor.Services
{
    public class AccountService
    {
        //private readonly IHttpClientFactory _clientFactory;
        //private readonly IAuthenticationService _authenticationService;

        //public AccountService(IHttpClientFactory clientFactory, IAuthenticationService authenticationService, DiscordSocketClient discordSocketClient)
        //{
        //    _clientFactory = clientFactory;
        //    _authenticationService = authenticationService;
        //}

        //public async Task<string> GetGuildIds(HttpContext context)
        //{
        //    var client = _clientFactory.CreateClient();
        //    var token = await _authenticationService.GetTokenAsync(context, "Discord");

        //    var rq = new HttpRequestMessage(HttpMethod.Get, "https://discordapp.com/api/users/@me/guilds");
        //    rq.Headers.Add("Authentication", token);
        //    var r = await client.SendAsync(rq);

        //    return await r.Content.ReadAsStringAsync();
        //}
    }
}
