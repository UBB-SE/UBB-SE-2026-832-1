using ClassLibrary.Data;
using ClassLibrary.IRepositories;
using ClassLibrary.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ClassLibrary.Extensions;

public static class ServiceCollectionExtensions
{
    private const string inMemoryDatabaseName = "AppDb";

    public static IServiceCollection AddClassLibraryDataAccess(this IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase(inMemoryDatabaseName));
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}

