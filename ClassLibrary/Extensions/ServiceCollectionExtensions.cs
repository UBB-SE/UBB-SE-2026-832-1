using ClassLibrary.Data;
using ClassLibrary.IRepositories;
using ClassLibrary.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ClassLibrary.Extensions;

public static class ServiceCollectionExtensions
{
    private const string IN_MEMORY_DATABASE_NAME = "AppDb";

    public static IServiceCollection AddClassLibraryDataAccess(this IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase(IN_MEMORY_DATABASE_NAME));
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<INutUserRepository, NutUserRepository>();
        services.AddScoped<IMealRepository, MealRepository>();
        services.AddScoped<IMealPlanRepository, MealPlanRepository>();
        services.AddScoped<IIngredientRepository, IngredientRepository>();
        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<IShoppingListRepository, ShoppingListRepository>();
        services.AddScoped<IReminderRepository, ReminderRepository>();
        services.AddScoped<IDailyLogRepository, DailyLogRepository>();
        services.AddScoped<IChatRepository, ChatRepository>();

        return services;
    }
}

