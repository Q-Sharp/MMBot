namespace MMBot.Blazor.Client.ViewModels;

public class MemberViewModel : ViewModelBase, ICRUDViewModel<MemberModel, Member>
{
    public string Entity => "Member";
    public IRepository<Member> Repo { get; set; }
    public MemberModel SelectedEntity { get; set; }
    public ICollection<MemberModel> Entities { get; set; }
    public ulong GID { get; set; }

    public MemberViewModel(IRepository<Member> repo, ISessionStorageService sessionStorage, IDialogService dialogService)
        : base(sessionStorage, dialogService)
    {
        Repo = repo;
        sessionStorage.Changing += (x, y) => _ = Init();

        Init().GetAwaiter().GetResult();
    }

    public async Task Init() => Entities = await Load(x => x.GuildId == GID, x => x.OrderBy(y => y.AHigh));

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

    public async Task Create(MemberModel newEntity)
    {
        try
        {
            var c = await Repo.Insert(newEntity);
            Entities.Add(c as MemberModel);
            SelectedEntity = c as MemberModel;
        }
        catch
        {

        }
    }

    public async Task<MemberModel> Update(MemberModel member)
    {
        if (member.Id == 0)
        {
            member.GuildId = GID;
            return await Repo.Insert(member) as MemberModel;
        }

        return await Repo.Update(member) as MemberModel;
    }

    public MemberModel Add()
    {
        var nte = new MemberModel();
        Entities.Add(nte);
        SelectedEntity = nte;
        OnPropertyChanged();
        return nte;
    }

    public async Task<IList<MemberModel>> Load(Expression<Func<Member, bool>> filter = null, Func<IQueryable<Member>, IOrderedQueryable<Member>> orderBy = null)
        => (await Repo.Get(filter, orderBy)).Select(x => MemberModel.Create(x)).ToList();
}
