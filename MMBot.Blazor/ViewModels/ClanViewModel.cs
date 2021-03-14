using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
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
        public Clan CurrentEntity { get; set; }
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
            await ReInit();
            (_accountService.LoggedUser as INotifyPropertyChanged).PropertyChanged += ClanViewModel_PropertyChanged;
        }

        private async Task ReInit()
        {
            var gid = _accountService.LoggedUser.CurrentGuildIdUlong;
            _guildId = gid.Value;
            Entities = await Load(x => x.GuildId == _guildId, x => x.OrderBy(y => y.SortOrder));
        }

        private async void ClanViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
           switch(e.PropertyName)
            {
                case "CurrentGuildId":
                    await ReInit();
                    break;
            }
        }

        public async Task Delete(int id)
        {
            var confirm = await _jSRuntime.InvokeAsync<bool>("confirm", "Do you want to delete this?");

            if (confirm)
            {
                try
                {
                    await _repo.Delete(id);
                    Entities.Remove(Entities.FirstOrDefault(x => x.Id == id));
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
            catch
            {

            }
        }

        public async Task<Clan> Update(Clan clan) => await _repo.Update(clan);

        public async Task<IList<Clan>> Load(Expression<Func<Clan, bool>> filter = null, Func<IQueryable<Clan>, IOrderedQueryable<Clan>> orderBy = null)
            => (await _repo.Get(filter,orderBy)).ToList();
    }
}
