using ClassLibrary.Data;
using ClassLibrary.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ClassLibrary.Extensions;

public static class DatabaseInitializer
{
    public static void SeedClassLibraryData(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        dbContext.Database.EnsureCreated();

        if (dbContext.Users.Any())
        {
            return;
        }

        dbContext.Users.AddRange(
            new User { Username = "nutritionist", Password = "admin123", Role = "Nutritionist" },
            new User { Username = "testuser", Password = "testpass1", Role = "User" });

        dbContext.Ingredients.AddRange(
            new Ingredient { IngredientId = 1, Name = "Chicken Breast", CaloriesPer100g = 165, ProteinPer100g = 31, CarbohydratesPer100g = 0, FatPer100g = 3.6 },
            new Ingredient { IngredientId = 2, Name = "Brown Rice", CaloriesPer100g = 216, ProteinPer100g = 5, CarbohydratesPer100g = 45, FatPer100g = 1.8 },
            new Ingredient { IngredientId = 3, Name = "Broccoli", CaloriesPer100g = 55, ProteinPer100g = 3.7, CarbohydratesPer100g = 11, FatPer100g = 0.6 });

        dbContext.FoodItems.AddRange(
            new FoodItem { Name = "Grilled Chicken Bowl", Calories = 450, Protein = 40, Carbohydrates = 35, Fat = 12 },
            new FoodItem { Name = "Veggie Stir Fry", Calories = 280, Protein = 12, Carbohydrates = 30, Fat = 8 });

        dbContext.SaveChanges();
    }
}
