using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Discord;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;

namespace MMBot.Blazor.Services
{
    public class AccountService : IAccountService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public AccountService(IHttpContextAccessor httpContextAccessor, AuthenticationStateProvider authenticationStateProvider)
        {
            _httpContextAccessor = httpContextAccessor;
            _authenticationStateProvider = authenticationStateProvider;

            authenticationStateProvider.AuthenticationStateChanged += AuthenticationStateProvider_AuthenticationStateChanged;

            _ = SetLoggedUserAsync();
        }

        private async void AuthenticationStateProvider_AuthenticationStateChanged(Task<AuthenticationState> task)
        {
            await SetLoggedUserAsync();
        }

        //public DCUser LoggedUser => JsonSerializer.Deserialize<DCUser>(_httpContextAccessor.HttpContext?.Items["dcuser"] as string);

        public DCUser LoggedUser { get; set; }
        private async Task SetLoggedUserAsync()
        {
            var dcuser = new DCUser();

            try
            {
                var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
                var user = authState.User;

                if (user.Identity.IsAuthenticated)
                {
                    dcuser.Name = user.Identity.Name;
                    var ids =  user.Claims.FirstOrDefault(c => c.Type == "guilds")?.Value;

                    var o = new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                        IgnoreReadOnlyProperties = true,
                    };

                    var channel = JsonSerializer.Deserialize<IList<DCChannel>>(ids, o);

                    dcuser.Guilds = channel.Where(x => x.owner ||
                                                       x.PermissionFlags.HasFlag(GuildPermission.Administrator) ||  
                                                       x.PermissionFlags.HasFlag(GuildPermission.ManageChannels) || 
                                                       x.PermissionFlags.HasFlag(GuildPermission.ManageGuild) ||
                                                       x.PermissionFlags.HasFlag(GuildPermission.ManageRoles)).ToList();

                    dcuser.CurrentGuildId = dcuser.Guilds.FirstOrDefault().id; 
                }
            }
            catch
            {

            }
            
            LoggedUser = dcuser;
        }
    }
}
