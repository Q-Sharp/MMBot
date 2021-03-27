using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using MMBot.Blazor.Data;
using MMBot.Data.Interfaces;
using MMBot.Services.Interfaces;

namespace MMBot.Blazor.ViewModels
{
    public abstract class CRUDBaseViewModel<TEntity> : ViewModelBase, ICRUDViewModel<TEntity> 
        where TEntity : class, IHaveGuildId, IHaveId, new()
    {
        protected readonly IRepository<TEntity> _repo;
        protected readonly StateContainer _stateContainer;
        protected readonly IJSRuntime _jSRuntime;

        public TEntity SelectedEntity { get; set; }
        public ICollection<TEntity> Entities { get; set; }
        public TEntity CurrentEntity { get; set; }
        public abstract string Entity { get; }

        protected ulong gid => ulong.Parse(_stateContainer.SelectedGuildId);

        public CRUDBaseViewModel(IRepository<TEntity> repo, StateContainer stateContainer, IJSRuntime jSRuntime)
        {
            _repo = repo;
            _stateContainer = stateContainer;
            _jSRuntime = jSRuntime;

            stateContainer.OnChange += () => _ = Init();

            Init().GetAwaiter().GetResult();
        }

        public abstract Task Delete(int id);
        public abstract Task Init();
        public abstract Task Create(TEntity entity);

        public virtual TEntity Add()
        {
            var nte = new TEntity();
            Entities.Add(nte);
            SelectedEntity = nte;
            return nte;
        }

        public abstract Task<TEntity> Update(TEntity entity);
        public abstract Task<IList<TEntity>> Load(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null);
    }
}
