using ClassLibrary.Data;
using ClassLibrary.IRepositories;
using ClassLibrary.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ClassLibrary.Extensions;

public static class ServiceCollectionExtensions
{
    private const string IN_MEMORY_DATABASE_NAME = "AppDb";

    public static IServiceCollection AddClassLibraryDataAccess(this IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase(IN_MEMORY_DATABASE_NAME));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRepositoryAchievements, RepositoryAchievements>();
        services.AddScoped<IRepositoryNotification, RepositoryNotification>();
        services.AddScoped<IRepositoryNutrition, RepositoryNutrition>();
        services.AddScoped<IDailyLogRepository, DailyLogRepository>();

        return services;
    }
}

