using ClassLibrary.Repositories;
using Microsoft.Extensions.DependencyInjection;
using WebApi.Services;
using ClassLibrary.IRepositories;

namespace WebApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWebApiServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IShoppingListService, ShoppingListService>();
        return services;
    }
}
