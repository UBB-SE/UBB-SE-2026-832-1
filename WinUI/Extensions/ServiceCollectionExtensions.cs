using ClassLibrary.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using WinUI.IServices;
using WinUI.Services;
using WinUI.Services.Interfaces;
namespace WinUI.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWinUiServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserServiceProxy, UserServiceProxy>();
        services.AddHttpClient<IActiveWorkoutService, ActiveWorkoutService>();
        services.AddHttpClient<ICreateWorkoutService, CreateWorkoutService>();
        services.AddHttpClient<ITrainerDashboardService, TrainerDashboardService>();
        services.AddHttpClient<IAchievementsService, AchievementsService>();
        services.AddHttpClient<IRankShowcaseService, RankShowcaseService>();
        services.AddHttpClient<IAchievementsService, AchievementsService>(client =>
        {
            client.BaseAddress = new Uri($"{ApiBaseUrl.BASE_URL}/api/");
        });
        services.AddHttpClient<IMealPlanService, MealPlanService>();
        services.AddScoped<IDailyLogService, DailyLogService>();
        services.AddScoped<IDailyLogServiceProxy, DailyLogServiceProxy>();
        services.AddScoped<IWorkoutLogService, WorkoutLogService>();
        services.AddScoped<IWorkoutLogServiceProxy, WorkoutLogServiceProxy>();
        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<IRemindersService, RemindersService>();
        services.AddScoped<IDailyLogService, DailyLogService>();
        services.AddHttpClient<IShoppingListService, ShoppingListService>();
        services.AddScoped<IMealService, MealService>();

        return services;
    }
}
