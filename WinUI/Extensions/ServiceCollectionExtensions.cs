using System.Net;
using ClassLibrary.Models;
using Microsoft.Extensions.DependencyInjection;
using ClassLibrary.Proxies;
using ClassLibrary.Proxies.Interfaces;
namespace WinUI.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWinUiServices(this IServiceCollection services)
    {
        services.AddScoped<IUserProxy, UserProxy>();
        services.AddScoped<IUserProxy, UserProxy>();
        services.AddHttpClient<IActiveWorkoutProxy, ActiveWorkoutProxy>();
        services.AddHttpClient<ICreateWorkoutProxy, CreateWorkoutProxy>();
        services.AddHttpClient<ITrainerDashboardProxy, TrainerDashboardProxy>();
        services.AddHttpClient<IInventoryProxy, InventoryProxy>();
        services.AddHttpClient<IAchievementsProxy, AchievementsProxy>();
        services.AddHttpClient<IRankShowcaseProxy, RankShowcaseProxy>();
        services.AddHttpClient<IAchievementsProxy, AchievementsProxy>(client =>
        {
            client.BaseAddress = new Uri($"{ApiBaseUrl.BASE_URL}/api/");
        });
        services.AddHttpClient<IMealPlanProxy, MealPlanProxy>();
        services.AddScoped<IDailyLogProxy, DailyLogProxy>();
        services.AddScoped<IDailyLogProxy, DailyLogProxy>();
        services.AddHttpClient<IWorkoutLogProxy, WorkoutLogProxy>();
        services.AddScoped<IChatProxy, ChatProxy>();
        services.AddScoped<IRemindersProxy, RemindersProxy>();
        services.AddScoped<IDailyLogProxy, DailyLogProxy>();
        services.AddHttpClient<IShoppingListProxy, ShoppingListProxy>();

        return services;
    }
}

