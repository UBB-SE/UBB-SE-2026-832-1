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
            new NutUser { Username = "nutritionist1", Password = "password123", Role = "Nutritionist" },
            new NutUser { Username = "testuser", Password = "password123", Role = "User" });

        dbContext.Ingredients.AddRange(
            new Ingredient { FoodId = 1, Name = "Chicken Breast", CaloriesPer100g = 165, ProteinPer100g = 31, CarbsPer100g = 0, FatPer100g = 3.6 },
            new Ingredient { FoodId = 2, Name = "Brown Rice", CaloriesPer100g = 111, ProteinPer100g = 2.6, CarbsPer100g = 23, FatPer100g = 0.9 },
            new Ingredient { FoodId = 3, Name = "Broccoli", CaloriesPer100g = 34, ProteinPer100g = 2.8, CarbsPer100g = 7, FatPer100g = 0.4 },
            new Ingredient { FoodId = 4, Name = "Salmon", CaloriesPer100g = 208, ProteinPer100g = 20, CarbsPer100g = 0, FatPer100g = 13 },
            new Ingredient { FoodId = 5, Name = "Oats", CaloriesPer100g = 389, ProteinPer100g = 16.9, CarbsPer100g = 66.3, FatPer100g = 6.9 });

        dbContext.Meals.AddRange(
            new Meal { Name = "Grilled Chicken with Rice", Calories = 450, Carbs = 40, Fat = 10, Protein = 45, Description = "Healthy grilled chicken served with steamed brown rice" },
            new Meal { Name = "Salmon Bowl", Calories = 520, Carbs = 35, Fat = 22, Protein = 38, Description = "Fresh salmon with quinoa and roasted vegetables", IsKeto = true, IsGlutenFree = true },
            new Meal { Name = "Oatmeal Breakfast", Calories = 350, Carbs = 55, Fat = 8, Protein = 15, Description = "Hearty oats with berries and honey", IsVegan = true, IsLactoseFree = true });

        dbContext.SaveChanges();
    }
}

