namespace MMBot.Blazor.Shared;

public interface ICRUDViewModel<TEntityModel, TEntity>
    where TEntityModel : class, IHaveId, IHaveGuildId, ICreate, TEntity, new()
    where TEntity : class, IHaveId, IHaveGuildId, new()
{
    Task Create(TEntityModel newEntity);
    Task Delete();
    Task<IList<TEntityModel>> Load(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null);
    Task<TEntityModel> Update(TEntityModel updated);
    TEntityModel Add();

    Task Init();
    ICollection<TEntityModel> Entities { get; set; }
    TEntityModel SelectedEntity { get; set; }

    IRepository<TEntity> Repo { get; }
    string Entity => nameof(TEntity);

    ISelectedGuildService SelectedGuildService { get; }

    ulong GID { get; }
}
