using Microsoft.Extensions.DependencyInjection;
using WinUI.Services;

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
        services.AddScoped<IDailyLogService, DailyLogService>();
        services.AddScoped<IDailyLogServiceProxy, DailyLogServiceProxy>();
        return services;
    }
}
