using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Discord;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MMBot.Blazor.Data;
using MMBot.Blazor.Services;
using MMBot.Data.Entities;
using MMBot.Services.Interfaces;

namespace MMBot.Blazor.ViewModels
{
    public class ClanViewModel : ViewModelBase, ICRUDViewModel<ClanModel, Clan>
    {
        public string Entity => "Clan";

        public IRepository<Clan> Repo { get; set; }
        public StateContainer StateContainer { get; set; }
        public IJSRuntime JSRuntime { get; set; }

        public ClanModel SelectedEntity { get; set; }
        public ICollection<ClanModel> Entities { get; set; }
        public ClanModel CurrentEntity { get; set; }

        public ulong GID => ulong.Parse(StateContainer.SelectedGuildId);

        public ClanViewModel(IRepository<Clan> repo, StateContainer stateContainer, IJSRuntime jSRuntime)
        {
            Repo = repo;
            StateContainer = stateContainer;
            JSRuntime = jSRuntime;

            stateContainer.OnChange += () => _ = Init();

            Init().GetAwaiter().GetResult();
        }


        public async Task Init() => Entities = await Load(x => x.GuildId == GID, x => x.OrderBy(y => y.SortOrder));

        public async Task Delete(int id)
        {
            var confirm = await JSRuntime.InvokeAsync<bool>("confirm", "Do you want to delete this?");

            if (confirm)
            {
                try
                {
                    await Repo.Delete(id);
                    Entities.Remove(Entities.FirstOrDefault(x => x.Id == id));
                }
                catch
                {
                    //
                }
            }
        }

        public async Task Create(ClanModel newEntity)
        {
            try
            {
                var c = await Repo.Insert(newEntity);
                Entities.Add(c as ClanModel);
            }
            catch
            {

            }
        }

        public async Task<ClanModel> Update(ClanModel clan)
        {
            if (clan.Id == 0)
            {
                clan.GuildId = GID;
                return await Repo.Insert(clan) as ClanModel;
            }
                
            return await Repo.Update(clan) as ClanModel;
        }

        public async Task<IList<ClanModel>> Load(Expression<Func<Clan, bool>> filter = null, Func<IQueryable<Clan>, IOrderedQueryable<Clan>> orderBy = null)
            => (await Repo.Get(filter, orderBy)).Select(x => ClanModel.Create(x)).ToList();

        public ClanModel Add()
        {
            var nte = new ClanModel();
            Entities.Add(nte);
            SelectedEntity = nte;
            return nte;
        }
    }
}
