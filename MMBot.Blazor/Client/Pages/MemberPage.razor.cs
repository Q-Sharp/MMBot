namespace MMBot.Blazor.Client.Pages;

[Authorize]
public partial class MemberPage
{
    [Inject]
    public IAuthorizedAntiForgeryClientFactory AuthorizedAntiForgeryClientFactory { get; set; }

    [Inject]
    ICRUDViewModel<MemberModel, Member> MemberVM { get; set; }

    [Inject] IRepository<Member> Member { get; set; }

    private string searchString = "";

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
