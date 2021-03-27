using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using MMBot.Blazor.ViewModels;
using MMBot.Data;
using MMBot.Data.Entities;
using MMBot.Services.Database;
using MMBot.Services.Interfaces;

namespace MMBot.Blazor.Helpers
{
    public static class CRUDViewModelHelpers
    {
        public static IServiceCollection AddCRUDViewModels(this IServiceCollection serviceCollection)
        {
             serviceCollection.AddScoped<IRepository<Clan>, DataRepository<Clan, Context>>()
                              .AddScoped<IRepository<Member>, DataRepository<Member, Context>>()
                              .AddTransient<ICRUDViewModel<Clan>, ClanViewModel>()
                              .AddTransient<ICRUDViewModel<Member>, MemberViewModel>();

            return serviceCollection;
        }
    }
}
