namespace MMBot.Blazor.Client.ViewModels;

public class ViewModel<TEntityModel, TEntity> : ViewModelBase, ICRUDViewModel<TEntityModel, TEntity> 
    where TEntityModel : class, IHaveId, IHaveGuildId, ICreate, TEntity, new()
    where TEntity : class, IHaveId, IHaveGuildId, new()
{
    public string Entity => typeof(TEntity).Name;
    public IRepository<TEntity> Repo { get; set; }
    public TEntityModel SelectedEntity { get; set; }
    public ICollection<TEntityModel> Entities { get; set; }
    public ulong GID { get; set; }

    public ViewModel(IRepository<TEntity> repo, ISessionStorageService sessionStorage, IDialogService dialogService)
        : base(sessionStorage, dialogService)
    {
        Repo = repo;
        sessionStorage.Changing += async (x, y) =>
        {
            GID = await SessionStorage.GetItemAsync<ulong>(SessionStoreDefaults.GuildId);
            await Init();
        };
    }

    public async Task Init() => Entities = await Load(x => x.GuildId == GID, x => x.OrderBy(y => y.Id));

    public async Task Delete()
    {
        var confirm = await DialogService.ShowMessageBox("Warning", "Do you want to delete this record?", yesText: "Yes", noText: "No");

        if (confirm ?? false)
            try
            {
                var id = SelectedEntity.Id;
                await Repo.Delete(id);
                Entities.Remove(Entities.FirstOrDefault(x => x.Id == id));
            }
            catch
            {
                //
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
        catch
        {
            //
        }
    }

    public async Task<TEntityModel> Update(TEntityModel entity)
    {
        if (entity.Id == 0)
        {
            entity.GuildId = GID;
            return await Repo.Insert(entity) as TEntityModel;
        }

        return await Repo.Update(entity) as TEntityModel;
    }

    public TEntityModel Add()
    {
        var nte = new TEntityModel();
        Entities.Add(nte);
        SelectedEntity = nte;
        OnPropertyChanged();
        return nte;
    }

    public async Task<IList<TEntityModel>> Load(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null)
        => (await Repo.Get(filter, orderBy)).Select(x => new TEntityModel()).ToList();
    
}
