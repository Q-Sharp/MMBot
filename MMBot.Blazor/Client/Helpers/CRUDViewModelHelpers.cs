namespace MMBot.Blazor.Client.Helpers;

public static class CRUDViewModelHelpers
{
    public static IServiceCollection AddCRUDViewModels(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IRepository<Clan>, DataRepository<Clan>>()
                         .AddScoped<IRepository<Member>, DataRepository<Member>>()
                         .AddTransient<ICRUDViewModel<ClanModel, Clan>, ViewModel<ClanModel, Clan>>()
                         .AddTransient<ICRUDViewModel<MemberModel, Member>, ViewModel<MemberModel, Member>>();

        return serviceCollection;
    }
}
