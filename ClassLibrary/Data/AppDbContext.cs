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
    public DbSet<ShoppingItem> ShoppingItems { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Meal>()
            .Property(meal => meal.Ingredients)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<string>>(v) ?? new List<string>())
            .Metadata.SetValueComparer(new ValueComparer<List<string>>(
                (a, b) => (a ?? new List<string>()).SequenceEqual(b ?? new List<string>()),
                a => a.Aggregate(0, (h, x) => HashCode.Combine(h, x.GetHashCode())),
                a => a.ToList()));

        modelBuilder.Entity<Reminder>(entity =>
        {
            entity.Property(reminder => reminder.Name)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(reminder => reminder.Frequency)
                .HasMaxLength(50);
        });
    }
}