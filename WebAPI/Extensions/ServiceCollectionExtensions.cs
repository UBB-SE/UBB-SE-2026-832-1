using ClassLibrary.IRepositories;
using ClassLibrary.Repositories;
using Microsoft.Extensions.DependencyInjection;
using WebApi.IServices;
using WebApi.Services;
using WebAPI.IServices;
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
        services.AddScoped<IReminderService, ReminderService>();

        services.AddScoped<IInventoryService, InventoryService>();
        services.AddScoped<IShoppingListService, ShoppingListService>();
        services.AddScoped<IAnalyticsService, AnalyticsService>();
        services.AddScoped<INutritionPlanService, NutritionPlanService>();
        services.AddScoped<IWorkoutLogService, WorkoutLogService>();
        services.AddScoped<IEvaluationEngineService, EvaluationEngineService>();
        services.AddScoped<IDailyLogService, DailyLogService>();
        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<IWorkoutAnalyticsStore, WorkoutAnalyticsStore>();
        services.AddScoped<IAnalyticsDashboardRefreshBus, AnalyticsDashboardRefreshBus>();
        services.AddScoped<IWorkoutDataForwarder, WorkoutDataForwarder>();
        services.AddSingleton<IAnalyticsDashboardRefreshBus, AnalyticsDashboardRefreshBus>();


        return services;
    }
}