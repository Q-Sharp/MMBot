using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MMBot.Blazor.Services;
using MMBot.Data.Entities;
using MMBot.Services.Interfaces;

namespace MMBot.Blazor.ViewModels
{
    public class ClanViewModel : ICRUDViewModel<Clan>
    {
        private readonly IRepository<Clan> _repo;
        private readonly IAccountService _accountService;
        private readonly NavigationManager _navigationManager;
        private readonly IJSRuntime _jSRuntime;
        private ulong _guildId;

        public ICollection<Clan> Entities { get; set; }
        public string Entity => "Clan";

        public ClanViewModel(IRepository<Clan> repo, IAccountService accountService, NavigationManager navigationManager, IJSRuntime jSRuntime)
        {
            _repo = repo;
            _accountService = accountService;
            _navigationManager = navigationManager;
            _jSRuntime = jSRuntime;

            Init().GetAwaiter().GetResult();
        }

        public async Task Init()
        {
            var user = _accountService.LoggedUser;

            var gid = user.CurrentGuildIdUlong;
            if (gid != null)
            {
                _guildId = gid.Value;

                 Entities = (await _repo.Get(x => x.GuildId == _guildId,
                    x => x.OrderBy(y => y.SortOrder)/*,
                    string.Join(',', EntityHelper.GetHeader<Clan>())*/)).ToList();
            }
               
        }

        public async Task Delete(int? id)
        {
            var confirm = await _jSRuntime.InvokeAsync<bool>("confirm", "Do you want to delete this?");

            if (confirm && id.HasValue)
            {
                try
                {
                    await _repo.Delete(id.Value);
                    Entities.Remove(Entities.FirstOrDefault(x => x.Id == id.Value));
                }
                catch
                {
                    //
                }
            }
        }

        public async Task Create()
        {
            try
            {
                var c = new Clan { GuildId = _guildId };
                c = await _repo.Insert(c);
                Entities.Add(c);
            }
            catch (Exception e)
            {

            }
            finally
            {
                _navigationManager.NavigateTo("/clan");
            }
        }
    }
}
