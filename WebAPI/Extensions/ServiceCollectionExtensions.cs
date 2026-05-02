using ClassLibrary.Repositories;
using ClassLibrary.IRepositories;
using Microsoft.Extensions.DependencyInjection;
using WebAPI.IServices;
using WebAPI.Services;

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

        
        services.AddScoped<IReminderService, ReminderService>();

        
        services.AddScoped<IRepositoryTrainer, RepositoryTrainer>();

        services.AddScoped<IInventoryService, InventoryService>();
        services.AddScoped<IShoppingListService, ShoppingListService>();
        return services;
    }
}