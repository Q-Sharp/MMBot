using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MMBot.Data.Helpers;
using MMBot.Data.Interfaces;
using MMBot.Services.Interfaces;

namespace MMBot.Services.Database
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TDataContext"></typeparam>
    public class DataRepository<TEntity, TDataContext> : IRepository<TEntity> 
        where TEntity : class, IHaveId
        where TDataContext : DbContext
    {
        protected readonly TDataContext context;
        internal DbSet<TEntity> dbSet;
        private readonly  ILogger<IRepository<TEntity>> _logger;

        public DataRepository(TDataContext dataContext, ILogger<IRepository<TEntity>> logger)
        {
            context = dataContext;
            dbSet = context.Set<TEntity>();
            _logger = logger;
        }

         public async virtual Task<IEnumerable<TEntity>> Get(
             Expression<Func<TEntity, bool>> filter = null, 
             Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, 
             string IncludeProperties = "")
        {
            try
            {
                IQueryable<TEntity> q = dbSet;

                if(filter != null)
                    q = q.Where(filter);

                IncludeProperties.Split(",", StringSplitOptions.RemoveEmptyEntries).ForEach(x => q = q.Include(x));

                return orderBy != null ? orderBy(q).ToList() : await q.ToListAsync();
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message);
                return null;
            }
        }

        public async virtual Task<TEntity> GetById(object id) => await dbSet.FindAsync(id);

        public async virtual Task<bool> Delete(TEntity entityToDelete)
        {
            if(context.Entry(entityToDelete).State == EntityState.Detached)
                dbSet.Attach(entityToDelete);

            dbSet.Remove(entityToDelete);
            return await context.SaveChangesAsync() >= 1;
        }

        public async virtual Task<bool> Delete(object id)
        {
            var e = await dbSet.FindAsync(id);
            return await Delete(e);
        }
       
        public async virtual Task<TEntity> Insert(TEntity entity)
        {
            await dbSet.AddAsync(entity);
            await context.SaveChangesAsync();
            return entity;
        }

        public async virtual Task<TEntity> Update(TEntity entityToUpdate)
        {
            var dbSet = context.Set<TEntity>();
            dbSet.Attach(entityToUpdate);
            context.Entry(entityToUpdate).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return entityToUpdate;
        }
    }
}
