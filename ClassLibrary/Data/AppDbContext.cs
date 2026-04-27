using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; } = default!;

    public DbSet<NutUser> NutUsers { get; set; } = default!;

    public DbSet<UserData> UserData { get; set; } = default!;

    public DbSet<Meal> Meals { get; set; } = default!;

    public DbSet<MealPlan> MealPlans { get; set; } = default!;

    public DbSet<Ingredient> Ingredients { get; set; } = default!;

    public DbSet<Inventory> Inventories { get; set; } = default!;

    public DbSet<ShoppingItem> ShoppingItems { get; set; } = default!;

    public DbSet<Reminder> Reminders { get; set; } = default!;

    public DbSet<DailyLog> DailyLogs { get; set; } = default!;

    public DbSet<Conversation> Conversations { get; set; } = default!;

    public DbSet<Message> Messages { get; set; } = default!;

    public DbSet<Favorite> Favorites { get; set; } = default!;

    public DbSet<MealsIngredient> MealsIngredients { get; set; } = default!;

    public DbSet<MealPlanMeal> MealPlanMeals { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ingredient>()
            .HasKey(i => i.FoodId);

        modelBuilder.Entity<Favorite>()
            .HasKey(f => new { f.UserId, f.MealId });

        modelBuilder.Entity<MealsIngredient>()
            .HasKey(mi => new { mi.MealId, mi.FoodId });

        modelBuilder.Entity<MealPlanMeal>()
            .HasKey(mpm => new { mpm.MealPlanId, mpm.MealId });
    }
}

