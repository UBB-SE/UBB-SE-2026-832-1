using ClassLibrary.Data;
using ClassLibrary.Models;

namespace ClassLibrary.Extensions.Seeders;

public static class FoodItemSeeder
{
    public static void Seed(AppDbContext dbContext)
    {
        if (dbContext.FoodItems.Any())
        {
            return;
        }

        SeedFoodItems(dbContext);

        dbContext.SaveChanges();

        SeedFoodItemIngredients(dbContext);

        dbContext.SaveChanges();
    }

    private static void SeedFoodItems(AppDbContext dbContext)
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
               },

               // ?? MEDIUM (550–700 kcal) ?? good for well-being & cut variety
               new FoodItem
               {
                   Name = "Chicken Caesar Salad",
                   Calories = 550,
                   Protein = 42,
                   Carbohydrates = 30,
                   Fat = 22,
                   IsGlutenFree = false,
                   IsNutFree = true
               },
               new FoodItem
               {
                   Name = "Baked Cod with Vegetables",
                   Calories = 480,
                   Protein = 48,
                   Carbohydrates = 38,
                   Fat = 10,
                   IsGlutenFree = true,
                   IsLactoseFree = true,
                   IsNutFree = true
               },
               new FoodItem
               {
                   Name = "Chickpea & Spinach Curry",
                   Calories = 560,
                   Protein = 20,
                   Carbohydrates = 72,
                   Fat = 18,
                   IsVegan = true,
                   IsGlutenFree = true,
                   IsNutFree = true
               },
               new FoodItem
               {
                   Name = "Whole Wheat Pasta with Turkey Meatballs",
                   Calories = 640,
                   Protein = 45,
                   Carbohydrates = 68,
                   Fat = 18,
                   IsNutFree = true
               },

               // ?? HIGH (750–900 kcal) ?? good for maintenance
               new FoodItem
               {
                   Name = "Pasta Bolognese",
                   Calories = 760,
                   Protein = 40,
                   Carbohydrates = 88,
                   Fat = 22,
                   IsNutFree = true
               },
               new FoodItem
               {
                   Name = "Beef Steak with Roasted Potatoes",
                   Calories = 820,
                   Protein = 62,
                   Carbohydrates = 58,
                   Fat = 30,
                   IsGlutenFree = true,
                   IsLactoseFree = true,
                   IsNutFree = true
               },
               new FoodItem
               {
                   Name = "Chicken Alfredo Pasta",
                   Calories = 800,
                   Protein = 50,
                   Carbohydrates = 78,
                   Fat = 28,
                   IsNutFree = true
               },
               new FoodItem
               {
                   Name = "BBQ Pulled Pork Rice Bowl",
                   Calories = 870,
                   Protein = 52,
                   Carbohydrates = 88,
                   Fat = 24,
                   IsGlutenFree = true,
                   IsNutFree = true
               },
               new FoodItem
               {
                   Name = "Peanut Butter & Banana Oatmeal",
                   Calories = 750,
                   Protein = 22,
                   Carbohydrates = 92,
                   Fat = 28,
                   IsVegan = true,
                   IsGlutenFree = true
               },

               // ?? VERY HIGH (950–1100 kcal) ?? good for bulk
               new FoodItem
               {
                   Name = "Full English Breakfast",
                   Calories = 950,
                   Protein = 55,
                   Carbohydrates = 62,
                   Fat = 48,
                   IsNutFree = true
               },
               new FoodItem
               {
                   Name = "Loaded Breakfast Burrito",
                   Calories = 980,
                   Protein = 50,
                   Carbohydrates = 92,
                   Fat = 40,
                   IsNutFree = true
               },
               new FoodItem
               {
                   Name = "Cheesy Beef Pasta Bake",
                   Calories = 1050,
                   Protein = 58,
                   Carbohydrates = 98,
                   Fat = 38,
                   IsNutFree = true
               },
               new FoodItem
               {
                   Name = "Mass Gainer Protein Bowl",
                   Calories = 1100,
                   Protein = 70,
                   Carbohydrates = 118,
                   Fat = 32,
                   IsGlutenFree = true
               },
               new FoodItem
               {
                   Name = "Double Cheeseburger & Sweet Potato Fries",
                   Calories = 1020,
                   Protein = 58,
                   Carbohydrates = 96,
                   Fat = 42,
                   IsNutFree = true
               },

               // ?? VERY LOW (100–180 kcal) ?? for aggressive cut diversity
               new FoodItem
               {
                   Name = "Mixed Green Salad with Lemon Dressing",
                   Calories = 110,
                   Protein = 3,
                   Carbohydrates = 10,
                   Fat = 6,
                   IsVegan = true,
                   IsGlutenFree = true,
                   IsLactoseFree = true,
                   IsNutFree = true
               },
               new FoodItem
               {
                   Name = "Cucumber & Hummus Plate",
                   Calories = 140,
                   Protein = 6,
                   Carbohydrates = 14,
                   Fat = 7,
                   IsVegan = true,
                   IsGlutenFree = true,
                   IsLactoseFree = true,
                   IsNutFree = true
               },
               new FoodItem
               {
                   Name = "Apple & Walnut Snack",
                   Calories = 170,
                   Protein = 4,
                   Carbohydrates = 22,
                   Fat = 8,
                   IsVegan = true,
                   IsGlutenFree = true,
                   IsLactoseFree = true
               });
    }

    private static void SeedFoodItemIngredients(AppDbContext dbContext)
    {
        if (dbContext.FoodItemIngredients.Any())
        {
            return;
        }

        Link(
            dbContext,
            "Grilled Chicken Bowl",
            ("Chicken Breast", 180),
            ("Brown Rice", 150),
            ("Broccoli", 100));

        Link(
            dbContext,
            "Veggie Stir Fry",
            ("Bell Pepper", 100),
            ("Broccoli", 120),
            ("Garlic", 10),
            ("Soy Sauce", 20));

        Link(
            dbContext,
            "Salmon & Quinoa",
            ("Salmon", 180),
            ("Quinoa", 140),
            ("Broccoli", 100));

        Link(
            dbContext,
            "Turkey Wrap",
            ("Turkey Breast", 150),
            ("Whole Wheat Tortilla", 80),
            ("Lettuce", 40),
            ("Tomato", 50));

        Link(
            dbContext,
            "Greek Yogurt & Berries",
            ("Greek Yogurt", 200),
            ("Mixed Berries", 80));

        Link(
            dbContext,
            "Oatmeal with Banana",
            ("Oats", 80),
            ("Banana", 120));

        Link(
            dbContext,
            "Beef & Sweet Potato",
            ("Ground Beef", 200),
            ("Sweet Potato", 250));

        Link(
            dbContext,
            "Tuna Salad",
            ("Tuna", 150),
            ("Olive Oil", 15),
            ("Cucumber", 100));

        Link(
            dbContext,
            "Lentil Soup",
            ("Lentils", 180),
            ("Carrot", 60),
            ("Onion", 50));

        Link(
            dbContext,
            "Egg White Omelette",
            ("Egg Whites", 180),
            ("Spinach", 60));

        Link(
            dbContext,
            "Avocado Toast",
            ("Avocado", 100),
            ("Whole Grain Bread", 80));

        Link(
            dbContext,
            "Protein Smoothie",
            ("Protein Powder", 40),
            ("Banana", 120),
            ("Almond Milk", 250));

        Link(
            dbContext,
            "Shrimp Fried Rice",
            ("Shrimp", 150),
            ("White Rice", 180),
            ("Egg", 60));

        Link(
            dbContext,
            "Cottage Cheese & Pineapple",
            ("Cottage Cheese", 200),
            ("Pineapple", 100));

        Link(
            dbContext,
            "Black Bean Burrito Bowl",
            ("Black Beans", 180),
            ("Brown Rice", 150),
            ("Salsa", 50));

        Link(
            dbContext,
            "Chicken Caesar Salad",
            ("Chicken Breast", 180),
            ("Romaine Lettuce", 120),
            ("Parmesan Cheese", 30),
            ("Caesar Dressing", 40),
            ("Croutons", 40));

        Link(
            dbContext,
            "Baked Cod with Vegetables",
            ("Cod Fillet", 220),
            ("Asparagus", 120),
            ("Potato", 180));

        Link(
            dbContext,
            "Chickpea & Spinach Curry",
            ("Chickpeas", 200),
            ("Spinach", 80),
            ("Coconut Milk", 120),
            ("Curry Paste", 30));

        Link(
            dbContext,
            "Whole Wheat Pasta with Turkey Meatballs",
            ("Turkey Breast", 180),
            ("Whole Wheat Tortilla", 100),
            ("Tomato Sauce", 120));

        Link(
            dbContext,
            "Pasta Bolognese",
            ("Pasta", 220),
            ("Ground Beef", 180),
            ("Tomato Sauce", 150),
            ("Onion", 50),
            ("Carrot", 50),
            ("Celery", 40));

        Link(
            dbContext,
            "Beef Steak with Roasted Potatoes",
            ("Ground Beef", 250),
            ("Potato", 250));

        Link(
            dbContext,
            "Chicken Alfredo Pasta",
            ("Chicken Breast", 200),
            ("Pasta", 220),
            ("Heavy Cream", 80),
            ("Butter", 20),
            ("Parmesan Cheese", 30));

        Link(
            dbContext,
            "BBQ Pulled Pork Rice Bowl",
            ("Pork Shoulder", 220),
            ("BBQ Sauce", 50),
            ("White Rice", 180),
            ("Coleslaw", 100));

        Link(
            dbContext,
            "Peanut Butter & Banana Oatmeal",
            ("Oats", 100),
            ("Peanut Butter", 40),
            ("Banana", 120),
            ("Honey", 20));

        Link(
            dbContext,
            "Full English Breakfast",
            ("Egg", 120),
            ("Bacon", 80),
            ("Sausage", 120),
            ("Baked Beans", 150),
            ("White Bread", 80));

        Link(
            dbContext,
            "Loaded Breakfast Burrito",
            ("Egg", 120),
            ("Cheddar Cheese", 50),
            ("Hash Browns", 150),
            ("Whole Wheat Tortilla", 100));

        Link(
            dbContext,
            "Cheesy Beef Pasta Bake",
            ("Ground Beef", 220),
            ("Pasta", 250),
            ("Mozzarella", 80),
            ("Bechamel Sauce", 120));

        Link(
            dbContext,
            "Mass Gainer Protein Bowl",
            ("Protein Powder", 60),
            ("Granola", 150),
            ("Peanuts", 60),
            ("Banana", 120),
            ("Dried Mango", 80));

        Link(
            dbContext,
            "Double Cheeseburger & Sweet Potato Fries",
            ("Beef Patty", 250),
            ("Cheddar Cheese", 40),
            ("Burger Bun", 100),
            ("Sweet Potato", 250));

        Link(
            dbContext,
            "Mixed Green Salad with Lemon Dressing",
            ("Mixed Greens", 120),
            ("Lemon Juice", 20),
            ("Olive Oil", 15),
            ("Cherry Tomatoes", 80));

        Link(
            dbContext,
            "Cucumber & Hummus Plate",
            ("Cucumber", 150),
            ("Hummus", 80));

        Link(
            dbContext,
            "Apple & Walnut Snack",
            ("Apple", 150),
            ("Walnuts", 40));

        dbContext.SaveChanges();
    }

    private static void Link(
     AppDbContext dbContext,
     string foodItemName,
     params (string ingredientName, double quantityGrams)[] ingredients)
    {
        FoodItem foodItem = dbContext.FoodItems
            .First(foodItemEntity => foodItemEntity.Name == foodItemName);

        foreach ((string ingredientName, double quantityGrams) ingredientData in ingredients)
        {
            Ingredient ingredient = dbContext.Ingredients
                .First(ingredientEntity => ingredientEntity.Name == ingredientData.ingredientName);

            dbContext.FoodItemIngredients.Add(
                new FoodItemIngredient
                {
                    FoodItem = foodItem,
                    Ingredient = ingredient,
                    QuantityGrams = ingredientData.quantityGrams
                });
        }
    }
}