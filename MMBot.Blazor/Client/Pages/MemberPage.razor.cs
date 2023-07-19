namespace MMBot.Blazor.Client.Pages;

[Authorize]
public partial class MemberPage
{
    [Inject]
    public IAuthorizedAntiForgeryClientFactory AuthorizedAntiForgeryClientFactory { get; set; }

    [Inject]
    private ICRUDViewModel<MemberModel, Member> MemberVM { get; set; }

    [CascadingParameter]
    public DCChannel SelectedGuild { get; set; } = new DCChannel();

    private string searchString = "";

    protected override async Task OnParametersSetAsync() => await MemberVM.Load();

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
