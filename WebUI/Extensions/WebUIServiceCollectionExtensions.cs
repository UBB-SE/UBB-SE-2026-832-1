using ClassLibrary.Proxies;
using ClassLibrary.Proxies.Interfaces;
using WebUI.Services;

namespace WebUI.Extensions;

public static class WebUIServiceCollectionExtensions
{
    public static IServiceCollection AddWebUiServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddDistributedMemoryCache();
        services.AddSession(options =>
        {
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        services.AddHttpClient<IClientDashboardProxy, ClientDashboardProxy>();
        services.AddScoped<IUserSession, HttpContextUserSession>();

        return services;
    }
}
