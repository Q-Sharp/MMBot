namespace MMBot.Blazor.Client.Services;

public class DataRepository<TEntity> : IRepository<TEntity>
    where TEntity : class, IHaveId, new()
{
    private readonly ILogger<IRepository<TEntity>> _logger;
    private readonly ISessionStorageService _sessionStorage;
    private readonly IAuthorizedAntiForgeryClientFactory _clientFactory;

    private async Task<ulong> GetGuildId() 
        => await _sessionStorage.GetItemAsync<ulong>(SessionStoreDefaults.GuildId);

    public DataRepository(IAuthorizedAntiForgeryClientFactory clientFactory, ILogger<IRepository<TEntity>> logger, ISessionStorageService sessionStorage)
    {
        _clientFactory = clientFactory;
        _logger = logger;
        _sessionStorage = sessionStorage;
    }

    public async virtual Task<IEnumerable<TEntity>> Get(
        Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        string includeProperties = "")
    {
        try
        {
            var http = await _clientFactory.CreateClient();
            var guildId = await GetGuildId();



            var q = (await http.GetAllEntities<TEntity>(guildId)).AsQueryable();

            if (filter != null)
                q = q.Where(filter);

            //includeProperties.Split(",", StringSplitOptions.RemoveEmptyEntries);

            return orderBy != null ? orderBy(q).ToList() : q.ToList();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return null;
        }
    }

    public async virtual Task<TEntity> GetById(object id)
    {
        var http = await _clientFactory.CreateClient();
        return await http.GetEntity<TEntity>((ulong)id);
    }

    public async virtual Task<bool> Delete(TEntity entityToDelete) 
        => await Delete(entityToDelete.Id);

    public async virtual Task<bool> Delete(object id)
    {
        var http = await _clientFactory.CreateClient();
        await http.DeleteEntity<TEntity>((ulong)id);

        return true;
    }

    public async virtual Task<TEntity> Insert(TEntity entity)
    {
        try
        {
            var http = await _clientFactory.CreateClient();
            return await http.CreateEntity(entity);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Couldn't insert {entity}", entity.ToString());
        }

        return default;
    }

    public async virtual Task<TEntity> Update(TEntity entityToUpdate)
    {
        try
        {
            var http = await _clientFactory.CreateClient();
            return await http.UpdateEntity(entityToUpdate);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Couldn't update {entityToUpdate}", entityToUpdate.ToString());
        }

        return default;
    }
}

