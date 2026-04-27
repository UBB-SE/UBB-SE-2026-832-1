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
                Id = Guid.NewGuid(),
                Email = SEED_USER_EMAIL,
                FullName = SEED_USER_FULL_NAME
            });

        dbContext.NutUsers.AddRange(
            new NutUser { Username = "nutritionist", Password = "admin123", Role = "nutritionist" },
            new NutUser { Username = "testuser", Password = "pass", Role = "regular" });

        dbContext.Ingredients.AddRange(
            new Ingredient { FoodId = 1, Name = "Chicken Breast", Calories = 165, Protein = 31, Carbs = 0, Fat = 3.6 },
            new Ingredient { FoodId = 2, Name = "Brown Rice", Calories = 216, Protein = 5, Carbs = 45, Fat = 1.8 },
            new Ingredient { FoodId = 3, Name = "Broccoli", Calories = 55, Protein = 3.7, Carbs = 11, Fat = 0.6 });

        dbContext.Meals.AddRange(
            new Meal { Name = "Grilled Chicken Bowl", CookingTime = 25, Servings = 2, CookingLevel = "Easy" },
            new Meal { Name = "Veggie Stir Fry", CookingTime = 15, Servings = 1, CookingLevel = "Easy" });

        dbContext.SaveChanges();
    }
}

