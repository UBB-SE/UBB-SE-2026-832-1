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

        var testUser = new User { Username = "testuser", Password = "testpass1", Role = "User" };
        dbContext.Users.AddRange(
            new User { Username = "nutritionist", Password = "admin123", Role = "Nutritionist" },
            testUser);

        var testClient = new Client
        {
            User = testUser,
            Email = "testuser@vibecoders.local",
            FullName = "Test Client",
            Weight = 75,
            Height = 175,
            PrimaryGoal = "General Fitness",
        };
        dbContext.Clients.Add(testClient);

        dbContext.WorkoutTemplates.AddRange(
            new WorkoutTemplate
            {
                Client = testClient,
                Name = "HIIT Fat Burner",
                Type = WorkoutType.PREBUILT,
                Exercises = new List<TemplateExercise>
                {
                    new() { Name = "Burpees", MuscleGroup = MuscleGroup.CARDIO, TargetSets = 4, TargetReps = 15 },
                    new() { Name = "Mountain Climbers", MuscleGroup = MuscleGroup.CARDIO, TargetSets = 4, TargetReps = 20 },
                    new() { Name = "Jump Squats", MuscleGroup = MuscleGroup.LEGS, TargetSets = 4, TargetReps = 15 },
                },
            },
            new WorkoutTemplate
            {
                Client = testClient,
                Name = "Full Body Mass",
                Type = WorkoutType.PREBUILT,
                Exercises = new List<TemplateExercise>
                {
                    new() { Name = "Bench Press", MuscleGroup = MuscleGroup.CHEST, TargetSets = 4, TargetReps = 8 },
                    new() { Name = "Deadlift", MuscleGroup = MuscleGroup.BACK, TargetSets = 4, TargetReps = 6 },
                    new() { Name = "Squat", MuscleGroup = MuscleGroup.LEGS, TargetSets = 4, TargetReps = 8 },
                    new() { Name = "Overhead Press", MuscleGroup = MuscleGroup.SHOULDERS, TargetSets = 3, TargetReps = 8 },
                },
            },
            new WorkoutTemplate
            {
                Client = testClient,
                Name = "Full Body Power",
                Type = WorkoutType.PREBUILT,
                Exercises = new List<TemplateExercise>
                {
                    new() { Name = "Power Clean", MuscleGroup = MuscleGroup.BACK, TargetSets = 5, TargetReps = 3 },
                    new() { Name = "Squat", MuscleGroup = MuscleGroup.LEGS, TargetSets = 5, TargetReps = 5 },
                    new() { Name = "Bench Press", MuscleGroup = MuscleGroup.CHEST, TargetSets = 5, TargetReps = 5 },
                },
            },
            new WorkoutTemplate
            {
                Client = testClient,
                Name = "Endurance Circuit",
                Type = WorkoutType.PREBUILT,
                Exercises = new List<TemplateExercise>
                {
                    new() { Name = "Push Ups", MuscleGroup = MuscleGroup.CHEST, TargetSets = 3, TargetReps = 20 },
                    new() { Name = "Pull Ups", MuscleGroup = MuscleGroup.BACK, TargetSets = 3, TargetReps = 12 },
                    new() { Name = "Plank", MuscleGroup = MuscleGroup.CORE, TargetSets = 3, TargetReps = 60 },
                    new() { Name = "Lunges", MuscleGroup = MuscleGroup.LEGS, TargetSets = 3, TargetReps = 20 },
                },
            });

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
