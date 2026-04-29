using ClassLibrary.Repositories;
using ClassLibrary.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using WebAPI.Services;

namespace WebAPI.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWebApiServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRepositoryTrainer, RepositoryTrainer>();

        return services;
    }
}