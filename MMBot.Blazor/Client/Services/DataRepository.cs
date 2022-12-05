namespace MMBot.Blazor.Client.Services;

public class DataRepository<TEntity> : IRepository<TEntity>
    where TEntity : class, IHaveId, new()
{
    private readonly ILogger<IRepository<TEntity>> _logger;
    private readonly ISelectedGuildService _selectedGuildService;
    private readonly IAuthorizedAntiForgeryClientFactory _clientFactory;

    private string _typeName => GetType().GetGenericArguments()[0].Name;

    private async Task<ulong> GetGuildId()
        => await _selectedGuildService.GetSelectedGuildId();

    public DataRepository(IAuthorizedAntiForgeryClientFactory clientFactory, ILogger<IRepository<TEntity>> logger, ISelectedGuildService selectedGuildService)
    {
        _clientFactory = clientFactory;
        _logger = logger;
        _selectedGuildService = selectedGuildService;
    }

    public virtual async Task<IEnumerable<TEntity>> Get(
        Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        string includeProperties = "")
    {
        try
        {
            var http = await _clientFactory.CreateClient();
            var guildId = await GetGuildId();

            var q = (await http.GetAllEntities<TEntity>(guildId, _typeName)).AsQueryable();

            if (filter != null)
                q = q.Where(filter);

             includeProperties.Split(",", StringSplitOptions.RemoveEmptyEntries);

            return orderBy != null ? orderBy(q).ToList() : q.ToList();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return null;
        }
    }

    public virtual async Task<TEntity> GetById(object id)
    {
        var http = await _clientFactory.CreateClient();
        return await http.GetEntity<TEntity>((ulong)id, _typeName);
    }

    public virtual async Task<bool> Delete(TEntity entityToDelete)
        => await Delete(entityToDelete.Id);

    public virtual async Task<bool> Delete(object id)
    {
        var http = await _clientFactory.CreateClient();
        await http.DeleteEntity<TEntity>((ulong)id, _typeName);

        return true;
    }

    public virtual async Task<TEntity> Insert(TEntity entity)
    {
        try
        {
            var http = await _clientFactory.CreateClient();
            return await http.CreateEntity(entity, _typeName);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Couldn't insert {entity}", entity.ToString());
        }

        return default;
    }

    public virtual async Task<TEntity> Update(TEntity entityToUpdate)
    {
        try
        {
            var http = await _clientFactory.CreateClient();
            return await http.UpdateEntity(entityToUpdate, _typeName);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Couldn't update {entityToUpdate}", entityToUpdate.ToString());
        }

        return default;
    }
}
