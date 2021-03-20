using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MMBot.Data.Interfaces;

namespace MMBot.Blazor.ViewModels
{
    public interface ICRUDViewModel<TEntity> where TEntity : new()
    {
        Task Create(TEntity newEntity);
        Task Delete(int id);
        Task<IList<TEntity>> Load(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null);
        Task<TEntity> Update(TEntity updated);
        string Entity { get; }
        ICollection<TEntity> Entities { get; }
        TEntity CurrentEntity { get; }
    }
}
