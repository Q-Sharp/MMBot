namespace MMBot.Blazor.Client.Pages;

[Authorize]
public partial class MemberPage
{
    [Inject]
    public IAuthorizedAntiForgeryClientFactory AuthorizedAntiForgeryClientFactory { get; set; }

    [Inject]
    private ICRUDViewModel<MemberModel, Member> MemberVM { get; set; }

    private string searchString = "";

    protected override async Task OnInitializedAsync() => await MemberVM.Init();

    private bool FilterFunc(MemberModel Member)
    {
        if (string.IsNullOrWhiteSpace(searchString))
            return true;
        if (Member.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        //if (Member.Tag.Contains(searchString, StringComparison.OrdinalIgnoreCase))
        //    return true;
        return false;
    }
}
