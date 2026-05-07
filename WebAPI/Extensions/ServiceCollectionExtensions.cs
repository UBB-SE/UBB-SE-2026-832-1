using ClassLibrary.IRepositories;
using ClassLibrary.Repositories;
using Microsoft.Extensions.DependencyInjection;
using WebApi.IServices;
using WebApi.Services;
using WebAPI.IServices;
using WebAPI.Services;
using WebAPI.Services.AchievementBus;
using WebAPI.Services.AchievementBus.Interfaces;

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
        services.AddScoped<IInventoryService, InventoryService>();
        services.AddScoped<IShoppingListService, ShoppingListService>();
        services.AddScoped<IAnalyticsService, AnalyticsService>();
        services.AddScoped<INutritionPlanService, NutritionPlanService>();
        services.AddScoped<IWorkoutLogService, WorkoutLogService>();
        services.AddScoped<IProgressionService, ProgressionService>();
        services.AddScoped<IEvaluationEngineService, EvaluationEngineService>();
        services.AddScoped<IDailyLogService, DailyLogService>();
        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<ICalendarWorkoutCatalogService, CalendarWorkoutCatalogService>();
        services.AddSingleton<IAchievementUnlockedBus, AchievementUnlockedBus>();
        services.AddScoped<IMealService, MealService>();

        return services;
    }
}