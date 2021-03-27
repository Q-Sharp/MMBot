using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MMBot.Blazor.Data;
using MMBot.Blazor.Services;
using MMBot.Data.Entities;
using MMBot.Services.Interfaces;

namespace MMBot.Blazor.ViewModels
{
    public class ClanViewModel : CRUDBaseViewModel<Clan>
    {
        public override string Entity => "Clan";

        public ClanViewModel(IRepository<Clan> repo, StateContainer stateContainer, IJSRuntime jSRuntime)
            : base(repo, stateContainer, jSRuntime)
        {

        }


        public async override Task Init() => Entities = await Load(x => x.GuildId == gid, x => x.OrderBy(y => y.SortOrder));

        public async override Task Delete(int id)
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

        public async override Task Create(Clan newEntity)
        {
            try
            {
                var c = await _repo.Insert(newEntity);
                Entities.Add(c);
            }
            catch
            {

            }
        }

        public async override Task<Clan> Update(Clan clan) 
            => clan.Id switch
            {
                0 => await _repo.Insert(clan),
                _ => await _repo.Update(clan),
            };

        public async override Task<IList<Clan>> Load(Expression<Func<Clan, bool>> filter = null, Func<IQueryable<Clan>, IOrderedQueryable<Clan>> orderBy = null)
            => (await _repo.Get(filter,orderBy)).ToList();
    }
}
