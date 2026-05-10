using ClassLibrary.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using WinUI.Services;
using WinUI.Services.Interfaces;
using WinUI.ViewModels;
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
        services.AddHttpClient<IWorkoutLogService, WorkoutLogService>();
        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<IRemindersService, RemindersService>();
        services.AddScoped<IDailyLogService, DailyLogService>();
        services.AddHttpClient<IRemindersService, RemindersService>(client =>
        {
            client.BaseAddress = new Uri(ApiBaseUrl.BASE_URL);
        });
        services.AddTransient<RemindersViewModel>();
        services.AddHttpClient<IShoppingListService, ShoppingListService>();
       
        return services;
    }
}
