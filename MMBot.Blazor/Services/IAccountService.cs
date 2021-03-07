namespace MMBot.Blazor.Services
{
    public interface IAccountService
    {
        IDCUser LoggedUser { get; }
    }
}