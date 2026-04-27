using ClassLibrary.Data;
using ClassLibrary.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ClassLibrary.Extensions;

public static class DatabaseInitializer
{
    private const string seedUserEmail = "test@example.com";
    private const string seedUserFullName = "test user";

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
                Email = seedUserEmail,
                FullName = seedUserFullName
            });
        dbContext.SaveChanges();
    }
}

