using System.Text.Json;
using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; } = default!;

    public DbSet<UserData> UserData { get; set; } = default!;

    public DbSet<FoodItem> FoodItems { get; set; } = default!;

    public DbSet<MealPlan> MealPlans { get; set; } = default!;

    public DbSet<Ingredient> Ingredients { get; set; } = default!;

    public DbSet<Inventory> Inventories { get; set; } = default!;

    public DbSet<Achievement> Achievements { get; set; } = default!;

    public DbSet<ClientAchievement> ClientAchievements { get; set; } = default!;

    public DbSet<WorkoutLog> WorkoutLogs { get; set; } = default!;

    public DbSet<Notification> Notifications { get; set; } = default!;

    public DbSet<NutritionPlan> NutritionPlans { get; set; } = default!;

    public DbSet<Meal> Meals { get; set; } = default!;

    public DbSet<ClientNutritionPlan> ClientNutritionPlans { get; set; } = default!;

    public DbSet<Conversation> Conversations { get; set; } = default!;

    public DbSet<Message> Messages { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ClientAchievement>()
            .HasKey(ca => new { ca.ClientId, ca.AchievementId });

        modelBuilder.Entity<ClientAchievement>()
            .HasOne(ca => ca.Achievement)
            .WithMany(a => a.ClientAchievements)
            .HasForeignKey(ca => ca.AchievementId);

        modelBuilder.Entity<NutritionPlan>()
            .HasKey(np => np.PlanId);

        modelBuilder.Entity<ClientNutritionPlan>()
            .HasKey(cnp => new { cnp.ClientId, cnp.NutritionPlanId });

        modelBuilder.Entity<ClientNutritionPlan>()
            .HasOne(cnp => cnp.NutritionPlan)
            .WithMany()
            .HasForeignKey(cnp => cnp.NutritionPlanId);

        modelBuilder.Entity<Meal>()
            .Property(m => m.Ingredients)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>());

        modelBuilder.Entity<Conversation>()
            .HasOne(c => c.User)
            .WithMany()
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Conversation>()
            .HasMany(c => c.Messages)
            .WithOne(m => m.Conversation)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Message>()
            .HasOne(m => m.Sender)
            .WithMany()
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}
