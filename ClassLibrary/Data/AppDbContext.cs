using System.Text.Json;
using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; } = default!;

    public DbSet<Achievement> Achievements { get; set; } = default!;

    public DbSet<ClientAchievement> ClientAchievements { get; set; } = default!;

    public DbSet<WorkoutLog> WorkoutLogs { get; set; } = default!;

    public DbSet<Notification> Notifications { get; set; } = default!;

    public DbSet<NutritionPlan> NutritionPlans { get; set; } = default!;

    public DbSet<Meal> Meals { get; set; } = default!;

    public DbSet<ClientNutritionPlan> ClientNutritionPlans { get; set; } = default!;

    public DbSet<Ingredient> Ingredients { get; set; }

    public DbSet<Reminder> Reminders { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ClientAchievement>()
            .HasKey(clientAchievement => new { clientAchievement.ClientId, clientAchievement.AchievementId });

        modelBuilder.Entity<ClientAchievement>()
            .HasOne(clientAchievement => clientAchievement.Achievement)
            .WithMany(achievement => achievement.ClientAchievements)
            .HasForeignKey(clientAchievement => clientAchievement.AchievementId);

        modelBuilder.Entity<NutritionPlan>()
            .HasKey(nutritionPlan => nutritionPlan.PlanId);

        modelBuilder.Entity<ClientNutritionPlan>()
            .HasKey(clientNutritionPlan => new { clientNutritionPlan.ClientId, clientNutritionPlan.NutritionPlanId });

        modelBuilder.Entity<ClientNutritionPlan>()
            .HasOne(clientNutritionPlan => clientNutritionPlan.NutritionPlan)
            .WithMany()
            .HasForeignKey(clientNutritionPlan => clientNutritionPlan.NutritionPlanId);

        modelBuilder.Entity<Meal>()
            .Property(meal => meal.Ingredients)
            .HasConversion(
                jsonValue => JsonSerializer.Serialize(jsonValue, (JsonSerializerOptions?)null),
                jsonValue => JsonSerializer.Deserialize<List<string>>(jsonValue, (JsonSerializerOptions?)null) ?? new List<string>());

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
