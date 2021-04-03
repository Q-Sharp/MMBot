using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using MMBot.Blazor.Data;
using MMBot.Data.Entities;
using MMBot.Data.Interfaces;
using MMBot.Services.Interfaces;
using static MudBlazor.Defaults;

namespace MMBot.Blazor.ViewModels
{
    public interface ICRUDViewModel<TEntityModel, TEntity>
        where TEntityModel : class, IHaveGuildId, IHaveId, new()
        where TEntity : class, IHaveGuildId, IHaveId, new()
    {
        Task Create(TEntityModel newEntity);
        Task Delete(int id);
        Task<IList<TEntityModel>> Load(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null);
        Task<TEntityModel> Update(TEntityModel updated);
        TEntityModel Add();

        Task Init();
        ICollection<TEntityModel> Entities { get; set;  }
        TEntityModel CurrentEntity { get; set; }

        TEntityModel SelectedEntity { get; set; }

        IRepository<TEntity> Repo { get; }
        StateContainer StateContainer { get; }
        IJSRuntime JSRuntime { get; }

        string Entity { get; }

        ulong GID { get; }
    }
}
