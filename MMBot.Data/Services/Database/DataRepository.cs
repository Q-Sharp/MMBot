using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MMBot.Data.Helpers;
using MMBot.Data.Interfaces;
using MMBot.Data.Services.Interfaces;

namespace MMBot.Data.Services.Database;

public class DataRepository<TEntity, TDataContext> : IRepository<TEntity>
    where TEntity : class, IHaveId, new()
    where TDataContext : DbContext
{
    protected readonly TDataContext context;
    internal DbSet<TEntity> dbSet;
    private readonly ILogger<IRepository<TEntity>> _logger;

    public DataRepository(TDataContext dataContext, ILogger<IRepository<TEntity>> logger)
    {
        context = dataContext;
        dbSet = context.Set<TEntity>();
        _logger = logger;
    }

    public async virtual Task<IEnumerable<TEntity>> Get(
        Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        string includeProperties = "")
    {
        try
        {
            IQueryable<TEntity> q = dbSet;

            if (filter != null)
                q = q.Where(filter);

            includeProperties.Split(",", StringSplitOptions.RemoveEmptyEntries).ForEach(x => q = q.Include(x));

            return orderBy != null ? orderBy(q).ToList() : await q.ToListAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return null;
        }
    }

    public async virtual Task<TEntity> GetById(object id) => await dbSet.FindAsync(id);

    public async virtual Task<bool> Delete(TEntity entityToDelete)
    {
        if (context.Entry(entityToDelete).State == EntityState.Detached)
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
        try
        {
            var e = dbSet.Add(entity);
            e.State = EntityState.Added;

            await context.SaveChangesAsync();
            return e.Entity;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Couldn't insert {0}", entity.ToString());
        }

        return null;
    }

    public async virtual Task<TEntity> Update(TEntity entityToUpdate)
    {
        var ex = false;

        try
        {
            dbSet.Attach(entityToUpdate);
            context.Entry(entityToUpdate).State = EntityState.Modified;
        }
        catch (Exception e)
        {
            ex = true;
            _logger.LogError(e, "Couldn't update {0}", entityToUpdate.ToString());
        }

        if (ex)
            try
            {
                var db = dbSet.FirstOrDefault(x => x.Id == entityToUpdate.Id);
                db.Update(entityToUpdate);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Still couldn't update {0}", entityToUpdate.ToString());
            }

        await context.SaveChangesAsync();
        return entityToUpdate;
    }
}
