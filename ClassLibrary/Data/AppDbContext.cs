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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ingredient>()
            .HasKey(i => i.FoodId);
    }
}

