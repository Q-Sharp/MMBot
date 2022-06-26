namespace MMBot.Blazor.Client.Helpers;

public static class CRUDViewModelHelpers
{
    public static IServiceCollection AddCRUDViewModels(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IRepository<Clan>, DataRepository<Clan>>()
                         .AddScoped<IRepository<Member>, DataRepository<Member>>()
                         .AddTransient<ICRUDViewModel<ClanModel, Clan>, ClanViewModel>()
                         .AddTransient<ICRUDViewModel<MemberModel, Member>, MemberViewModel>();

        return serviceCollection;
    }
}
