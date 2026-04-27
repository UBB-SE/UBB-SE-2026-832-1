using ClassLibrary.Data;
using ClassLibrary.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ClassLibrary.Extensions;

public static class DatabaseInitializer
{
    private const string SeedUserEmail = "test@example.com";
    private const string SeedUserFullName = "test user";

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
                Email = SeedUserEmail,
                FullName = SeedUserFullName
            });
        dbContext.SaveChanges();
    }
}

