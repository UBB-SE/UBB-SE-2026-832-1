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

        // USER DATA
        if (!dbContext.UserData.Any())
        {
            var nutritionistUser = dbContext.Users.First(u => u.Username == "nutritionist");
            var testUserEntity = dbContext.Users.First(u => u.Username == "testuser");

            dbContext.UserData.AddRange(
                new UserData
                {
                    User = nutritionistUser,
                    Weight = 80,
                    Height = 178,
                    Age = 30,
                    Gender = "male",
                    Goal = "maintenance",
                    BodyMassIndex = 25.2,
                    CalorieNeeds = 2739,
                    ProteinNeeds = 150,
                    CarbohydrateNeeds = 343,
                    FatNeeds = 91
                },
                new UserData
                {
                    User = testUserEntity,
                    Weight = 82,
                    Height = 180,
                    Age = 25,
                    Gender = "male",
                    Goal = "bulk",
                    BodyMassIndex = 25.3,
                    CalorieNeeds = 3129,
                    ProteinNeeds = 180,
                    CarbohydrateNeeds = 391,
                    FatNeeds = 104
                });

            dbContext.SaveChanges();
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
                // Grilled Chicken Bowl
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
                },

                // Veggie Stir Fry
                new Ingredient
                {
                    Name = "Bell Pepper",
                    CaloriesPer100g = 31,
                    ProteinPer100g = 1,
                    CarbohydratesPer100g = 6,
                    FatPer100g = 0.3
                },
                new Ingredient
                {
                    Name = "Garlic",
                    CaloriesPer100g = 149,
                    ProteinPer100g = 6.4,
                    CarbohydratesPer100g = 33,
                    FatPer100g = 0.5
                },
                new Ingredient
                {
                    Name = "Soy Sauce",
                    CaloriesPer100g = 53,
                    ProteinPer100g = 8.1,
                    CarbohydratesPer100g = 4.9,
                    FatPer100g = 0.1
                },

                // Salmon & Quinoa
                new Ingredient
                {
                    Name = "Salmon",
                    CaloriesPer100g = 208,
                    ProteinPer100g = 20,
                    CarbohydratesPer100g = 0,
                    FatPer100g = 13
                },
                new Ingredient
                {
                    Name = "Quinoa",
                    CaloriesPer100g = 120,
                    ProteinPer100g = 4.4,
                    CarbohydratesPer100g = 21,
                    FatPer100g = 1.9
                },

                // Turkey Wrap
                new Ingredient
                {
                    Name = "Turkey Breast",
                    CaloriesPer100g = 135,
                    ProteinPer100g = 30,
                    CarbohydratesPer100g = 0,
                    FatPer100g = 1
                },
                new Ingredient
                {
                    Name = "Whole Wheat Tortilla",
                    CaloriesPer100g = 290,
                    ProteinPer100g = 9,
                    CarbohydratesPer100g = 50,
                    FatPer100g = 6
                },
                new Ingredient
                {
                    Name = "Lettuce",
                    CaloriesPer100g = 15,
                    ProteinPer100g = 1.4,
                    CarbohydratesPer100g = 2.9,
                    FatPer100g = 0.2
                },
                new Ingredient
                {
                    Name = "Tomato",
                    CaloriesPer100g = 18,
                    ProteinPer100g = 0.9,
                    CarbohydratesPer100g = 3.9,
                    FatPer100g = 0.2
                },

                // Greek Yogurt & Berries
                new Ingredient
                {
                    Name = "Greek Yogurt",
                    CaloriesPer100g = 97,
                    ProteinPer100g = 9,
                    CarbohydratesPer100g = 6,
                    FatPer100g = 5
                },
                new Ingredient
                {
                    Name = "Mixed Berries",
                    CaloriesPer100g = 57,
                    ProteinPer100g = 0.7,
                    CarbohydratesPer100g = 14,
                    FatPer100g = 0.3
                },

                // Oatmeal with Banana
                new Ingredient
                {
                    Name = "Oats",
                    CaloriesPer100g = 389,
                    ProteinPer100g = 17,
                    CarbohydratesPer100g = 66,
                    FatPer100g = 7
                },
                new Ingredient
                {
                    Name = "Banana",
                    CaloriesPer100g = 89,
                    ProteinPer100g = 1.1,
                    CarbohydratesPer100g = 23,
                    FatPer100g = 0.3
                },

                // Beef & Sweet Potato
                new Ingredient
                {
                    Name = "Ground Beef",
                    CaloriesPer100g = 254,
                    ProteinPer100g = 26,
                    CarbohydratesPer100g = 0,
                    FatPer100g = 17
                },
                new Ingredient
                {
                    Name = "Sweet Potato",
                    CaloriesPer100g = 86,
                    ProteinPer100g = 1.6,
                    CarbohydratesPer100g = 20,
                    FatPer100g = 0.1
                },

                // Tuna Salad
                new Ingredient
                {
                    Name = "Tuna",
                    CaloriesPer100g = 132,
                    ProteinPer100g = 28,
                    CarbohydratesPer100g = 0,
                    FatPer100g = 1.3
                },
                new Ingredient
                {
                    Name = "Olive Oil",
                    CaloriesPer100g = 884,
                    ProteinPer100g = 0,
                    CarbohydratesPer100g = 0,
                    FatPer100g = 100
                },
                new Ingredient
                {
                    Name = "Cucumber",
                    CaloriesPer100g = 16,
                    ProteinPer100g = 0.7,
                    CarbohydratesPer100g = 3.6,
                    FatPer100g = 0.1
                },

                // Lentil Soup
                new Ingredient
                {
                    Name = "Lentils",
                    CaloriesPer100g = 116,
                    ProteinPer100g = 9,
                    CarbohydratesPer100g = 20,
                    FatPer100g = 0.4
                },
                new Ingredient
                {
                    Name = "Carrot",
                    CaloriesPer100g = 41,
                    ProteinPer100g = 0.9,
                    CarbohydratesPer100g = 10,
                    FatPer100g = 0.2
                },
                new Ingredient
                {
                    Name = "Onion",
                    CaloriesPer100g = 40,
                    ProteinPer100g = 1.1,
                    CarbohydratesPer100g = 9.3,
                    FatPer100g = 0.1
                },

                // Egg White Omelette
                new Ingredient
                {
                    Name = "Egg Whites",
                    CaloriesPer100g = 52,
                    ProteinPer100g = 11,
                    CarbohydratesPer100g = 0.7,
                    FatPer100g = 0.2
                },
                new Ingredient
                {
                    Name = "Spinach",
                    CaloriesPer100g = 23,
                    ProteinPer100g = 2.9,
                    CarbohydratesPer100g = 3.6,
                    FatPer100g = 0.4
                },

                // Avocado Toast
                new Ingredient
                {
                    Name = "Avocado",
                    CaloriesPer100g = 160,
                    ProteinPer100g = 2,
                    CarbohydratesPer100g = 9,
                    FatPer100g = 15
                },
                new Ingredient
                {
                    Name = "Whole Grain Bread",
                    CaloriesPer100g = 247,
                    ProteinPer100g = 13,
                    CarbohydratesPer100g = 41,
                    FatPer100g = 4.2
                },

                // Protein Smoothie
                new Ingredient
                {
                    Name = "Protein Powder",
                    CaloriesPer100g = 380,
                    ProteinPer100g = 75,
                    CarbohydratesPer100g = 10,
                    FatPer100g = 5
                },
                new Ingredient
                {
                    Name = "Almond Milk",
                    CaloriesPer100g = 17,
                    ProteinPer100g = 0.6,
                    CarbohydratesPer100g = 1.5,
                    FatPer100g = 1.1
                },

                // Shrimp Fried Rice
                new Ingredient
                {
                    Name = "Shrimp",
                    CaloriesPer100g = 99,
                    ProteinPer100g = 24,
                    CarbohydratesPer100g = 0.2,
                    FatPer100g = 0.3
                },
                new Ingredient
                {
                    Name = "White Rice",
                    CaloriesPer100g = 130,
                    ProteinPer100g = 2.7,
                    CarbohydratesPer100g = 28,
                    FatPer100g = 0.3
                },
                new Ingredient
                {
                    Name = "Egg",
                    CaloriesPer100g = 155,
                    ProteinPer100g = 13,
                    CarbohydratesPer100g = 1.1,
                    FatPer100g = 11
                },

                // Cottage Cheese & Pineapple
                new Ingredient
                {
                    Name = "Cottage Cheese",
                    CaloriesPer100g = 98,
                    ProteinPer100g = 11,
                    CarbohydratesPer100g = 3.4,
                    FatPer100g = 4.3
                },
                new Ingredient
                {
                    Name = "Pineapple",
                    CaloriesPer100g = 50,
                    ProteinPer100g = 0.5,
                    CarbohydratesPer100g = 13,
                    FatPer100g = 0.1
                },

                // Black Bean Burrito Bowl
                new Ingredient
                {
                    Name = "Black Beans",
                    CaloriesPer100g = 132,
                    ProteinPer100g = 8.9,
                    CarbohydratesPer100g = 24,
                    FatPer100g = 0.5
                },
                new Ingredient
                {
                    Name = "Salsa",
                    CaloriesPer100g = 36,
                    ProteinPer100g = 1.7,
                    CarbohydratesPer100g = 7,
                    FatPer100g = 0.3
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
                    Fat = 12,
                    IsGlutenFree = true
                },
                new FoodItem
                {
                    Name = "Veggie Stir Fry",
                    Calories = 280,
                    Protein = 12,
                    Carbohydrates = 30,
                    Fat = 8,
                    IsVegan = true,
                    IsGlutenFree = true
                },
                new FoodItem
                {
                    Name = "Salmon & Quinoa",
                    Calories = 520,
                    Protein = 42,
                    Carbohydrates = 38,
                    Fat = 18,
                    IsGlutenFree = true,
                    IsLactoseFree = true
                },
                new FoodItem
                {
                    Name = "Turkey Wrap",
                    Calories = 380,
                    Protein = 30,
                    Carbohydrates = 40,
                    Fat = 10
                },
                new FoodItem
                {
                    Name = "Greek Yogurt & Berries",
                    Calories = 220,
                    Protein = 18,
                    Carbohydrates = 28,
                    Fat = 4,
                    IsGlutenFree = true,
                    IsNutFree = true
                },
                new FoodItem
                {
                    Name = "Oatmeal with Banana",
                    Calories = 310,
                    Protein = 10,
                    Carbohydrates = 58,
                    Fat = 5,
                    IsVegan = true,
                    IsLactoseFree = true
                },
                new FoodItem
                {
                    Name = "Beef & Sweet Potato",
                    Calories = 580,
                    Protein = 45,
                    Carbohydrates = 42,
                    Fat = 20,
                    IsGlutenFree = true,
                    IsLactoseFree = true
                },
                new FoodItem
                {
                    Name = "Tuna Salad",
                    Calories = 260,
                    Protein = 35,
                    Carbohydrates = 8,
                    Fat = 10,
                    IsGlutenFree = true,
                    IsLactoseFree = true,
                    IsNutFree = true
                },
                new FoodItem
                {
                    Name = "Lentil Soup",
                    Calories = 300,
                    Protein = 18,
                    Carbohydrates = 45,
                    Fat = 6,
                    IsVegan = true,
                    IsGlutenFree = true,
                    IsNutFree = true
                },
                new FoodItem
                {
                    Name = "Egg White Omelette",
                    Calories = 200,
                    Protein = 28,
                    Carbohydrates = 5,
                    Fat = 7,
                    IsGlutenFree = true,
                    IsLactoseFree = true,
                    IsNutFree = true
                },
                new FoodItem
                {
                    Name = "Avocado Toast",
                    Calories = 340,
                    Protein = 10,
                    Carbohydrates = 36,
                    Fat = 18,
                    IsVegan = true,
                    IsNutFree = true
                },
                new FoodItem
                {
                    Name = "Protein Smoothie",
                    Calories = 350,
                    Protein = 35,
                    Carbohydrates = 40,
                    Fat = 6,
                    IsGlutenFree = true,
                    IsNutFree = true
                },
                new FoodItem
                {
                    Name = "Shrimp Fried Rice",
                    Calories = 430,
                    Protein = 28,
                    Carbohydrates = 55,
                    Fat = 10,
                    IsLactoseFree = true,
                    IsNutFree = true
                },
                new FoodItem
                {
                    Name = "Cottage Cheese & Pineapple",
                    Calories = 190,
                    Protein = 22,
                    Carbohydrates = 20,
                    Fat = 3,
                    IsGlutenFree = true,
                    IsNutFree = true
                },
                new FoodItem
                {
                    Name = "Black Bean Burrito Bowl",
                    Calories = 460,
                    Protein = 20,
                    Carbohydrates = 65,
                    Fat = 12,
                    IsVegan = true,
                    IsNutFree = true
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
