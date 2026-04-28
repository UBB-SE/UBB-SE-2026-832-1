using ClassLibrary.Data;
using ClassLibrary.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ClassLibrary.Extensions;

public static class DatabaseInitializer
{
    private const string SEED_USER_EMAIL = "test@example.com";
    private const string SEED_USER_FULL_NAME = "test user";

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
                UserId = Guid.NewGuid(),
                Email = SEED_USER_EMAIL,
                FullName = SEED_USER_FULL_NAME
            });
        dbContext.SaveChanges();
    }
}

