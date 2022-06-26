namespace MMBot.Blazor.Client.ViewModels;

public class ClanViewModel : ViewModelBase, ICRUDViewModel<ClanModel, Clan>
{
    public IRepository<Clan> Repo { get; set; }
    public ClanModel SelectedEntity { get; set; }
    public ICollection<ClanModel> Entities { get; set; }
    public ulong GID { get; set; }

    public ClanViewModel(IRepository<Clan> repo, ISessionStorageService sessionStorage, IDialogService dialogService)
        : base(sessionStorage, dialogService)
    {
        Repo = repo;
        sessionStorage.Changing += async (x, y) =>
        {
            GID = await SessionStorage.GetItemAsync<ulong>(SessionStoreDefaults.GuildId);
            await Init();
        };
    }

    public async Task Init() => Entities = await Load(x => x.GuildId == GID, x => x.OrderBy(y => y.SortOrder));

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

    public async Task Create(ClanModel newEntity)
    {
        try
        {
            var c = await Repo.Insert(newEntity);
            Entities.Add(c as ClanModel);
            SelectedEntity = c as ClanModel;
        }
        catch
        {

        }
    }

    public async Task<ClanModel> Update(ClanModel clan)
    {
        if (clan.Id == 0)
        {
            clan.GuildId = GID;
            return await Repo.Insert(clan) as ClanModel;
        }

        return await Repo.Update(clan) as ClanModel;
    }

    public async Task<IList<ClanModel>> Load(Expression<Func<Clan, bool>> filter = null, Func<IQueryable<Clan>, IOrderedQueryable<Clan>> orderBy = null)
    {
        var fromAPI = await Repo.Get(filter, orderBy);

        return fromAPI.Select(x => ClanModel.Create(x)).ToList();
    }

    public ClanModel Add()
    {
        var nte = new ClanModel();
        Entities.Add(nte);
        SelectedEntity = nte;
        OnPropertyChanged();
        return nte;
    }
}
