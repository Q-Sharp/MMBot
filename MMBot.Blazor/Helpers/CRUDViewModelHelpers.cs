using MMBot.Blazor.ViewModels;
using MMBot.Data;
using MMBot.Data.Entities;
using MMBot.Services.Database;
using MMBot.Services.Interfaces;

namespace MMBot.Blazor.Helpers;

public static class CRUDViewModelHelpers
{
    public static IServiceCollection AddCRUDViewModels(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IRepository<Clan>, DataRepository<Clan, Context>>()
                         .AddScoped<IRepository<Member>, DataRepository<Member, Context>>()
                         .AddTransient<ICRUDViewModel<ClanModel, Clan>, ClanViewModel>()
                         .AddTransient<ICRUDViewModel<MemberModel, Member>, MemberViewModel>();

        return serviceCollection;
    }
}
