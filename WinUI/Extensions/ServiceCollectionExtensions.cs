using Microsoft.Extensions.DependencyInjection;
using WinUI.Services;

namespace WinUI.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWinUiServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserServiceProxy, UserServiceProxy>();
        services.AddScoped<IActiveWorkoutService, ActiveWorkoutService>();
        services.AddScoped<IAnotherService, AnotherService>();
        services.AddScoped<IAdditionalService, AdditionalService>();
        return services;
    }
}
