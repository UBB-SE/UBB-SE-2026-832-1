using Microsoft.Extensions.DependencyInjection;
using WebAPI.Services;
using WebAPI.Services.Interfaces;

namespace WebAPI.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWebApiServices(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IClientService, ClientService>();
        services.AddScoped<IFoodItemService, FoodItemService>();
        services.AddScoped<IMealPlanService, MealPlanService>();
        return services;
    }
}
