using MMBot.Blazor.Client.Services;
using MMBot.Blazor.Client.ViewModels;
using MMBot.Blazor.Shared;
using MMBot.Blazor.Shared.BusinessModel;
using MMBot.Data.Contracts.Entities;

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
