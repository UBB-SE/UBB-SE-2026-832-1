using System.Collections.Generic;
using System.Text.Json;
using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ClassLibrary.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = default!;
    public DbSet<UserData> UserData { get; set; } = default!;
    public DbSet<FoodItem> FoodItems { get; set; } = default!;
    public DbSet<MealPlan> MealPlans { get; set; } = default!;
    public DbSet<Ingredient> Ingredients { get; set; } = default!;
    public DbSet<Inventory> Inventories { get; set; } = default!;
    public DbSet<Favorite> Favorites { get; set; } = default!;
    public DbSet<FoodItemIngredient> FoodItemIngredients { get; set; } = default!;
    public DbSet<MealPlanFoodItem> MealPlanFoodItems { get; set; } = default!;
    public DbSet<Client> Clients { get; set; } = default!;
    public DbSet<WorkoutLog> WorkoutLogs { get; set; } = default!;
    public DbSet<WorkoutTemplate> WorkoutTemplates { get; set; } = default!;
    public DbSet<Notification> Notifications { get; set; } = default!;
    public DbSet<Reminder> Reminders { get; set; } = default!;
    public DbSet<NutritionPlan> NutritionPlans { get; set; } = default!;
    public DbSet<Meal> Meals { get; set; } = default!;
    public DbSet<ClientNutritionPlan> ClientNutritionPlans { get; set; } = default!;
    public DbSet<DailyLog> DailyLogs { get; set; } = default!;
    public DbSet<Conversation> Conversations { get; set; } = default!;
    public DbSet<Message> Messages { get; set; } = default!;
    public DbSet<ShoppingItem> ShoppingItems { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Favorite>(entity =>
        {
            entity.HasOne(favorite => favorite.User)
                .WithMany()
                .HasForeignKey("UserId")
                .IsRequired();

            entity.HasOne(favorite => favorite.FoodItem)
                .WithMany()
                .HasForeignKey("FoodItemId")
                .IsRequired();

            entity.HasIndex("UserId", "FoodItemId").IsUnique();
        });

        modelBuilder.Entity<FoodItemIngredient>(entity =>
        {
            entity.HasOne(foodItemIngredient => foodItemIngredient.FoodItem)
                .WithMany()
                .HasForeignKey("FoodItemId")
                .IsRequired();

            entity.HasOne(foodItemIngredient => foodItemIngredient.Ingredient)
                .WithMany()
                .HasForeignKey("IngredientId")
                .IsRequired();

            entity.HasIndex("FoodItemId", "IngredientId").IsUnique();
        });

        modelBuilder.Entity<MealPlan>(entity =>
        {
            entity.HasOne(mealPlan => mealPlan.User)
                .WithMany()
                .HasForeignKey("UserId")
                .IsRequired();
        });

        modelBuilder.Entity<MealPlanFoodItem>(entity =>
        {
            entity.HasOne(mealPlanFoodItem => mealPlanFoodItem.MealPlan)
                .WithMany()
                .HasForeignKey("MealPlanId")
                .IsRequired();

            entity.HasOne(mealPlanFoodItem => mealPlanFoodItem.FoodItem)
                .WithMany()
                .HasForeignKey("FoodItemId")
                .IsRequired();

            entity.HasIndex("MealPlanId", "FoodItemId").IsUnique();
        });

        modelBuilder.Entity<ClientNutritionPlan>()
            .HasKey("ClientId", "NutritionPlanId");

        modelBuilder.Entity<ClientNutritionPlan>()
            .HasOne(clientNutritionPlan => clientNutritionPlan.Client)
            .WithMany(client => client.ClientNutritionPlans)
            .HasForeignKey("ClientId");

        modelBuilder.Entity<ClientNutritionPlan>()
            .HasOne(clientNutritionPlan => clientNutritionPlan.NutritionPlan)
            .WithMany()
            .HasForeignKey("NutritionPlanId");

        
        modelBuilder.Entity<Meal>()
            .Property(meal => meal.Ingredients)
            .HasConversion(
                ingredientList => JsonSerializer.Serialize(ingredientList, (JsonSerializerOptions?)null),
                serializedIngredientList =>
                    JsonSerializer.Deserialize<List<string>>(serializedIngredientList, (JsonSerializerOptions?)null)
                    ?? new List<string>())
            .Metadata.SetValueComparer(new ValueComparer<List<string>>(
                (firstList, secondList) =>
                    (firstList ?? new List<string>()).SequenceEqual(secondList ?? new List<string>()),
                list =>
                    list.Aggregate(0, (combinedHash, currentItem) =>
                        HashCode.Combine(combinedHash, currentItem.GetHashCode())),
                list => list.ToList()));

        modelBuilder.Entity<Reminder>(entity =>
        {
            entity.Property(reminder => reminder.Name)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(reminder => reminder.Frequency)
                .HasMaxLength(50);

            entity.HasOne(reminder => reminder.User)
                .WithMany(user => user.Reminders)
                .IsRequired();
        });
    }
}