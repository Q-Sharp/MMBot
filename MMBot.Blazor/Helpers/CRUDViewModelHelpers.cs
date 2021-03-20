using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using MMBot.Blazor.ViewModels;
using MMBot.Data.Entities;

namespace MMBot.Blazor.Helpers
{
    public static class CRUDViewModelHelpers
    {
        public static IServiceCollection AddCRUDViewModels(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<ICRUDViewModel<Clan>, ClanViewModel>();
                             //.AddScoped<ICRUDViewModel<Member>, MemberViewModel>();

            return serviceCollection;
        }
    }
}
