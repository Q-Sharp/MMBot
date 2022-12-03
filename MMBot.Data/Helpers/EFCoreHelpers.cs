namespace MMBot.Data.Helpers;

public static class EFCoreHelpers
{
    public static IAsyncEnumerable<TEntity> AsAsyncEnumerable<TEntity>(this DbSet<TEntity> obj) where TEntity : class
        => EntityFrameworkQueryableExtensions.AsAsyncEnumerable(obj);

    public static Task<TEntity> FirstOrDefaultAsync<TEntity>(this DbSet<TEntity> obj) where TEntity : class
        => EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(obj);

    public static IQueryable<TEntity> Where<TEntity>(this DbSet<TEntity> obj, System.Linq.Expressions.Expression<Func<TEntity, bool>> predicate) where TEntity : class
        => Queryable.Where(obj, predicate);
}
