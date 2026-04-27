using ClassLibrary.Data;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using ClassLibrary.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ClassLibrary.DependencyInjection;

public static class ClassLibraryDataAccessExtensions
{
    public static IServiceCollection AddClassLibraryDataAccess(this IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase("AppDb"));
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }

    public static void SeedClassLibraryData(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (dbContext.Users.Any())
        {
            return;
        }

        dbContext.Users.Add(
            new User
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                FullName = "test user"
            });
        dbContext.SaveChanges();
    }
}

