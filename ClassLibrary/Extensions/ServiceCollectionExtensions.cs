using ClassLibrary.Data;
using ClassLibrary.IRepositories;
using ClassLibrary.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ClassLibrary.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddClassLibraryDataAccess(this IServiceCollection services)
    {
        // 1. Swapped InMemory for SQLite using your new DatabasePaths utility
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(DatabasePaths.GetConnectionString()));

        // 2. Kept ALL of the existing repository registrations so nothing breaks!
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRepositoryAchievements, RepositoryAchievements>();
        services.AddScoped<IRepositoryNotification, RepositoryNotification>();
        services.AddScoped<IRepositoryNutrition, RepositoryNutrition>();

        return services;
    }
}