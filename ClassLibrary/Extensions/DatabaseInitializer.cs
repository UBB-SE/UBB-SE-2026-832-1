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

        dbContext.SaveChanges();

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

            dbContext.WorkoutTemplates.AddRange(pushTemplate, pullTemplate);
        }

        // CHATS & MESSAGES
        if (!dbContext.Conversations.Any())
        {
            var nutritionist = dbContext.Users.First(u => u.Username == "nutritionist");

            var conversation = new Conversation
            {
                User = testUser!,
                HasUnanswered = true
            };
            dbContext.Conversations.Add(conversation);

            dbContext.Messages.AddRange(
                new Message
                {
                    Conversation = conversation,
                    Sender = testUser!,
                    TextContent = "Hi!",
                    SentAt = DateTime.UtcNow.AddHours(-2)
                },
                new Message
                {
                    Conversation = conversation,
                    Sender = nutritionist,
                    TextContent = "banana",
                    SentAt = DateTime.UtcNow.AddHours(-1)
                },
                new Message
                {
                    Conversation = conversation,
                    Sender = testUser!,
                    TextContent = "Thanks!",
                    SentAt = DateTime.UtcNow.AddMinutes(-5)
                }
            );

            var saraUser = new User
            {
                Username = "healthy_sara",
                Password = "password123",
                Role = "User"
            };
            dbContext.Users.Add(saraUser);
            dbContext.SaveChanges();

            var resolvedConversation = new Conversation
            {
                User = saraUser,
                HasUnanswered = false
            };
            dbContext.Conversations.Add(resolvedConversation);
            dbContext.SaveChanges();

            dbContext.Messages.AddRange(
                new Message
                {
                    Conversation = resolvedConversation,
                    Sender = saraUser,
                    TextContent = "no",
                    SentAt = DateTime.UtcNow.AddDays(-1)
                },
                new Message
                {
                    Conversation = resolvedConversation,
                    Sender = nutritionist,
                    TextContent = "Yes!yesyes",
                    SentAt = DateTime.UtcNow.AddDays(-1).AddHours(2)
                }
            );

            dbContext.SaveChanges();

            var newClient = new User
            {
                Username = "new_client",
                Password = "password123",
                Role = "User"
            };
            dbContext.Users.Add(newClient);
            dbContext.SaveChanges();

            var newConvo = new Conversation
            {
                User = newClient,
                HasUnanswered = true
            };
            dbContext.Conversations.Add(newConvo);

            dbContext.Messages.Add(new Message
            {
                Conversation = newConvo,
                Sender = newClient,
                TextContent = "Hello! I just joined and need a meal plan.",
                SentAt = DateTime.UtcNow
            });
        }

        dbContext.SaveChanges();
    }
}