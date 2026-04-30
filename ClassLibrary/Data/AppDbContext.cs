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

    public DbSet<Achievement> Achievements { get; set; } = default!;

    public DbSet<WorkoutLog> WorkoutLogs { get; set; } = default!;

    public DbSet<Notification> Notifications { get; set; } = default!;

    public DbSet<NutritionPlan> NutritionPlans { get; set; } = default!;

    public DbSet<Meal> Meals { get; set; } = default!;

    public DbSet<ClientNutritionPlan> ClientNutritionPlans { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Favorite>()
            .HasIndex(favorite => new { favorite.UserId, favorite.FoodItemId })
            .IsUnique();

        modelBuilder.Entity<FoodItemIngredient>()
            .HasIndex(foodItemIngredient => new { foodItemIngredient.FoodItemId, foodItemIngredient.IngredientId })
            .IsUnique();

        modelBuilder.Entity<MealPlanFoodItem>()
            .HasIndex(mealPlanFoodItem => new { mealPlanFoodItem.MealPlanId, mealPlanFoodItem.FoodItemId })
            .IsUnique();

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
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>());
    }
}
