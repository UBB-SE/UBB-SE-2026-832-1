using ClassLibrary.Data;
using ClassLibrary.IRepositories;
using ClassLibrary.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ClassLibrary.Extensions;

public static class DataAccessServiceCollectionExtensions
{
    public static IServiceCollection AddClassLibraryDataAccess(this IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(DatabasePaths.GetConnectionString()));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IFoodItemRepository, FoodItemRepository>();
        services.AddScoped<IMealPlanRepository, MealPlanRepository>();
        services.AddScoped<IRepositoryAchievements, RepositoryAchievements>();
        services.AddScoped<IRepositoryNotification, RepositoryNotification>();
        services.AddScoped<IRepositoryNutrition, RepositoryNutrition>();
        services.AddScoped<IChatRepository, ChatRepository>();
        services.AddScoped<IDailyLogRepository, DailyLogRepository>();
        services.AddScoped<IIngredientRepository, IngredientRepository>();
        services.AddScoped<IReminderRepository, ReminderRepository>();

        return services;
    }
}