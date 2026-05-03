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
    public DbSet<Achievement> Achievements { get; set; } = default!;
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

        modelBuilder.Entity<Meal>()
            .Property(meal => meal.Ingredients)
            .HasConversion(
                ingredientList => JsonSerializer.Serialize(ingredientList, (JsonSerializerOptions?)null),
                serializedIngredientList => JsonSerializer.Deserialize<List<string>>(serializedIngredientList, (JsonSerializerOptions?)null) ?? new List<string>())
            .Metadata.SetValueComparer(new ValueComparer<List<string>>(
                (firstIngredientList, secondIngredientList) =>
                    (firstIngredientList ?? new List<string>()).SequenceEqual(secondIngredientList ?? new List<string>()),
                ingredientList =>
                    ingredientList.Aggregate(0, (combinedHash, ingredient) =>
                        HashCode.Combine(combinedHash, ingredient.GetHashCode())),
                ingredientList => ingredientList.ToList()));

        modelBuilder.Entity<Reminder>()
            .Property(reminder => reminder.Name)
            .IsRequired()
            .HasMaxLength(255);

        modelBuilder.Entity<Reminder>()
            .Property(reminder => reminder.Frequency)
            .HasMaxLength(50);

        modelBuilder.Entity<Reminder>()
            .Ignore(reminder => reminder.FullDateTimeDisplay);

        modelBuilder.Entity<Reminder>()
            .HasOne(reminder => reminder.User)
            .WithMany(user => user.Reminders)
            .IsRequired();
    }
}