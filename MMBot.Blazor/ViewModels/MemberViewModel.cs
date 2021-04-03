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
    public class MemberViewModel : ViewModelBase, ICRUDViewModel<MemberModel, Member>
    {
        public string Entity => "Member";
        public IRepository<Member> Repo { get; set; }
        public StateContainer StateContainer { get; set; }
        public IJSRuntime JSRuntime { get; set; }

        public MemberModel SelectedEntity { get; set; }
        public ICollection<MemberModel> Entities { get; set; }
        public MemberModel CurrentEntity { get; set; }
        public ulong GID => ulong.Parse(StateContainer.SelectedGuildId);

        public MemberViewModel(IRepository<Member> repo, StateContainer stateContainer, IJSRuntime jSRuntime)
        {
            Repo = repo;
            StateContainer = stateContainer;
            JSRuntime = jSRuntime;

            stateContainer.OnChange += () => _ = Init();

            Init().GetAwaiter().GetResult();
        }

        public async Task Init() => Entities = await Load(x => x.GuildId == GID, x => x.OrderBy(y => y.AHigh));

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

        public async Task Create(MemberModel newEntity)
        {
            try
            {
                var c = await Repo.Insert(newEntity);
                Entities.Add(c as MemberModel);
            }
            catch
            {

            }
        }

        public MemberModel Add()
        {
            var nte = new MemberModel();
            Entities.Add(nte);
            SelectedEntity = nte;
            return nte;
        }

        public async Task<MemberModel> Update(MemberModel member) => await Repo.Update(member) as MemberModel;

        public async Task<IList<MemberModel>> Load(Expression<Func<Member, bool>> filter = null, Func<IQueryable<Member>, IOrderedQueryable<Member>> orderBy = null)
            => (await Repo.Get(filter, orderBy)).Select(x => x as MemberModel).ToList();
    }
}
