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

        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        User? testUser = null;

        // USERS
        if (!dbContext.Users.Any())
        {
            var nutritionistUser = new User
            {
                Username = "nutritionist",
                Password = "admin123",
                Role = "Nutritionist"
            };

            testUser = new User
            {
                Username = "testuser",
                Password = "testpass1",
                Role = "User"
            };

            dbContext.Users.AddRange(nutritionistUser, testUser);
        }
        else
        {
            testUser = dbContext.Users.First(user => user.Username == "testuser");
        }

        // CLIENTS
        if (!dbContext.Clients.Any())
        {
            dbContext.Clients.Add(
                new Client
                {
                    User = testUser!,
                    Email = "john.doe@test.com",
                    FullName = "John Doe",
                    Weight = 82,
                    Height = 180,
                    PrimaryGoal = "Build muscle and lose fat"
                });
        }

        // INGREDIENTS
        if (!dbContext.Ingredients.Any())
        {
            dbContext.Ingredients.AddRange(
                new Ingredient
                {
                    Name = "Chicken Breast",
                    CaloriesPer100g = 165,
                    ProteinPer100g = 31,
                    CarbohydratesPer100g = 0,
                    FatPer100g = 3.6
                },

                new Ingredient
                {
                    Name = "Brown Rice",
                    CaloriesPer100g = 216,
                    ProteinPer100g = 5,
                    CarbohydratesPer100g = 45,
                    FatPer100g = 1.8
                },

                new Ingredient
                {
                    Name = "Broccoli",
                    CaloriesPer100g = 55,
                    ProteinPer100g = 3.7,
                    CarbohydratesPer100g = 11,
                    FatPer100g = 0.6
                });
        }

        // FOOD ITEMS
        if (!dbContext.FoodItems.Any())
        {
            dbContext.FoodItems.AddRange(
                new FoodItem
                {
                    Name = "Grilled Chicken Bowl",
                    Calories = 450,
                    Protein = 40,
                    Carbohydrates = 35,
                    Fat = 12
                },

                new FoodItem
                {
                    Name = "Veggie Stir Fry",
                    Calories = 280,
                    Protein = 12,
                    Carbohydrates = 30,
                    Fat = 8
                });
        }

        dbContext.SaveChanges();

        // WORKOUT TEMPLATES
        if (!dbContext.WorkoutTemplates.Any())
        {
            var client = dbContext.Clients.First();

            var pushTemplate = new WorkoutTemplate
            {
                Client = client,
                Name = "Push Day Beginner",
                Type = WorkoutType.PREBUILT,

                Exercises = new List<TemplateExercise>
                {
                    new TemplateExercise
                    {
                        Name = "Bench Press",
                        MuscleGroup = MuscleGroup.CHEST,
                        TargetSets = 4,
                        TargetReps = 10,
                        TargetWeight = 60
                    },

                    new TemplateExercise
                    {
                        Name = "Shoulder Press",
                        MuscleGroup = MuscleGroup.SHOULDERS,
                        TargetSets = 3,
                        TargetReps = 12,
                        TargetWeight = 40
                    },

                    new TemplateExercise
                    {
                        Name = "Tricep Pushdown",
                        MuscleGroup = MuscleGroup.ARMS,
                        TargetSets = 3,
                        TargetReps = 15,
                        TargetWeight = 25
                    }
                }
            };

            var pullTemplate = new WorkoutTemplate
            {
                Client = client,
                Name = "Pull Day Intermediate",
                Type = WorkoutType.PREBUILT,

                Exercises = new List<TemplateExercise>
                {
                    new TemplateExercise
                    {
                        Name = "Deadlift",
                        MuscleGroup = MuscleGroup.BACK,
                        TargetSets = 5,
                        TargetReps = 5,
                        TargetWeight = 100
                    },

                    new TemplateExercise
                    {
                        Name = "Pull Up",
                        MuscleGroup = MuscleGroup.BACK,
                        TargetSets = 4,
                        TargetReps = 10,
                        TargetWeight = 0
                    },

                    new TemplateExercise
                    {
                        Name = "Barbell Curl",
                        MuscleGroup = MuscleGroup.ARMS,
                        TargetSets = 3,
                        TargetReps = 12,
                        TargetWeight = 20
                    }
                }
            };

            var hiitTemplate = new WorkoutTemplate
            {
                Client = client,
                Name = "HIIT Fat Burner",
                Type = WorkoutType.PREBUILT,
                Exercises = new List<TemplateExercise>
                {
                    new TemplateExercise { Name = "Burpees", MuscleGroup = MuscleGroup.CARDIO, TargetSets = 4, TargetReps = 15 },
                    new TemplateExercise { Name = "Mountain Climbers", MuscleGroup = MuscleGroup.CARDIO, TargetSets = 4, TargetReps = 20 },
                    new TemplateExercise { Name = "Jump Squats", MuscleGroup = MuscleGroup.LEGS, TargetSets = 4, TargetReps = 15 }
                }
            };

            var massTemplate = new WorkoutTemplate
            {
                Client = client,
                Name = "Full Body Mass",
                Type = WorkoutType.PREBUILT,
                Exercises = new List<TemplateExercise>
                {
                    new TemplateExercise { Name = "Bench Press", MuscleGroup = MuscleGroup.CHEST, TargetSets = 4, TargetReps = 8 },
                    new TemplateExercise { Name = "Deadlift", MuscleGroup = MuscleGroup.BACK, TargetSets = 4, TargetReps = 6 },
                    new TemplateExercise { Name = "Squat", MuscleGroup = MuscleGroup.LEGS, TargetSets = 4, TargetReps = 8 },
                    new TemplateExercise { Name = "Overhead Press", MuscleGroup = MuscleGroup.SHOULDERS, TargetSets = 3, TargetReps = 8 }
                }
            };

            var powerTemplate = new WorkoutTemplate
            {
                Client = client,
                Name = "Full Body Power",
                Type = WorkoutType.PREBUILT,
                Exercises = new List<TemplateExercise>
                {
                    new TemplateExercise { Name = "Power Clean", MuscleGroup = MuscleGroup.BACK, TargetSets = 5, TargetReps = 3 },
                    new TemplateExercise { Name = "Squat", MuscleGroup = MuscleGroup.LEGS, TargetSets = 5, TargetReps = 5 },
                    new TemplateExercise { Name = "Bench Press", MuscleGroup = MuscleGroup.CHEST, TargetSets = 5, TargetReps = 5 }
                }
            };

            var enduranceTemplate = new WorkoutTemplate
            {
                Client = client,
                Name = "Endurance Circuit",
                Type = WorkoutType.PREBUILT,
                Exercises = new List<TemplateExercise>
                {
                    new TemplateExercise { Name = "Push Ups", MuscleGroup = MuscleGroup.CHEST, TargetSets = 3, TargetReps = 20 },
                    new TemplateExercise { Name = "Pull Ups", MuscleGroup = MuscleGroup.BACK, TargetSets = 3, TargetReps = 12 },
                    new TemplateExercise { Name = "Plank", MuscleGroup = MuscleGroup.CORE, TargetSets = 3, TargetReps = 60 },
                    new TemplateExercise { Name = "Lunges", MuscleGroup = MuscleGroup.LEGS, TargetSets = 3, TargetReps = 20 }
                }
            };

            dbContext.WorkoutTemplates.AddRange(pushTemplate, pullTemplate, hiitTemplate, massTemplate, powerTemplate, enduranceTemplate);
        }

        dbContext.SaveChanges();
    }
}
