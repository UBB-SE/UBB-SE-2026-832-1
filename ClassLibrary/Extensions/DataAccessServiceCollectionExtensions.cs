using ClassLibrary.Data;
using ClassLibrary.IRepositories;
using ClassLibrary.Repositories;
using ClassLibrary.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ClassLibrary.Extensions;

public static class DataAccessServiceCollectionExtensions
{
    public static IServiceCollection AddClassLibraryDataAccess(this IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(DatabasePaths.GetConnectionString()));

        services.AddScoped<IReminderRepository, ReminderRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IFoodItemRepository, FoodItemRepository>();
        services.AddScoped<IMealPlanRepository, MealPlanRepository>();
        services.AddScoped<IAchievementsRepository, AchievementsRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<INutritionRepository, NutritionRepository>();
        services.AddScoped<IChatRepository, ChatRepository>();
        services.AddScoped<IDailyLogRepository, DailyLogRepository>();

        
        
        services.AddScoped<IRepositoryWorkoutLog, RepositoryWorkoutLog>();

        
        services.AddScoped<IWorkoutLogRepository, WorkoutLogRepository>();
        services.AddScoped<IWorkoutTemplateRepository, WorkoutTemplateRepository>();
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IIngredientRepository, IngredientRepository>();
        services.AddScoped<IShoppingListRepository, ShoppingListRepository>();
        services.AddScoped<IWorkoutAnalyticsRepository, WorkoutAnalyticsRepository>();

        return services;
    }
}