namespace MMBot.Blazor.Client.ViewModels;

public class ViewModel<TEntityModel, TEntity> : ViewModelBase, ICRUDViewModel<TEntityModel, TEntity>
    where TEntityModel : class, IHaveId, IHaveGuildId, ICreate, TEntity, new()
    where TEntity : class, IHaveId, IHaveGuildId, new()
{
    public string Entity => typeof(TEntity).Name;
    public IRepository<TEntity> Repo { get; set; }
    private readonly ILogger<ViewModel<TEntityModel, TEntity>> _logger;

    public TEntityModel SelectedEntity { get; set; }
    public ICollection<TEntityModel> Entities { get; set; }
    public ulong GID { get; set; }

    public ViewModel(IRepository<TEntity> repo, ISelectedGuildService selectedGuildService, IDialogService dialogService, ILogger<ViewModel<TEntityModel, TEntity>> logger)
        : base(selectedGuildService, dialogService)
    {
        Repo = repo;
        _logger = logger;
        selectedGuildService.Changed += async (_, _) => await Init();
    }

    public async Task Init()
    {
        GID = await SelectedGuildService.GetSelectedGuildId();
        Entities = await Load(x => x.GuildId == GID, x => x.OrderBy(y => y.Id));
    }

    public async Task Delete()
    {
        var confirm = await DialogService.ShowMessageBox("Warning", "Do you want to delete this record?", yesText: "Yes", noText: "No");

        if (confirm ?? false)
        {
            try
            {
                var id = SelectedEntity.Id;
                await Repo.Delete(id);
                Entities.Remove(Entities.FirstOrDefault(x => x.Id == id));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Delete error!");
            }
        }
    }

    public async Task Create(TEntityModel newEntity)
    {
        try
        {
            var c = await Repo.Insert(newEntity);
            Entities.Add(c as TEntityModel);
            SelectedEntity = c as TEntityModel;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Create error!");
        }
    }

    public async Task<TEntityModel> Update(TEntityModel entity)
    {
        try
        {
            if (entity.Id == 0)
            {
                entity.GuildId = GID;
                return await Repo.Insert(entity) as TEntityModel;
            }

            return await Repo.Update(entity) as TEntityModel;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Update error!");
        }

        return null;
    }

    public TEntityModel Add()
    {
        try
        {
            var nte = new TEntityModel();
            Entities.Add(nte);
            SelectedEntity = nte;
            OnPropertyChanged();
            return nte;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Update error!");
        }

        return new TEntityModel();
    }

    public async Task<IList<TEntityModel>> Load(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null)
        => (await Repo.Get(filter, orderBy))
              .Select(e =>
              {
                  var mi = typeof(TEntityModel).GetMethod("Create", BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod);
                  return mi.Invoke(null, new object[] { e }) as TEntityModel;
              })
              .ToList();
}
