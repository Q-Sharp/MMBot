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
    public class MemberViewModel : CRUDBaseViewModel<Member>
    {
        public override string Entity => "Member";

        public MemberViewModel(IRepository<Member> repo, StateContainer stateContainer, IJSRuntime jSRuntime)
            : base(repo, stateContainer, jSRuntime)
        {

        }

        public async override Task Init() => Entities = await Load(x => x.GuildId == gid, x => x.OrderBy(y => y.AHigh));

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

        public async override Task Create(Member newEntity)
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

        public async override Task<Member> Update(Member member) => await _repo.Update(member);

        public async override Task<IList<Member>> Load(Expression<Func<Member, bool>> filter = null, Func<IQueryable<Member>, IOrderedQueryable<Member>> orderBy = null)
            => (await _repo.Get(filter, orderBy)).ToList();
    }
}
