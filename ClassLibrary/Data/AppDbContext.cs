using System.Text.Json;
using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; } = default!;

    public DbSet<Client> Clients { get; set; } = default!;

    public DbSet<Achievement> Achievements { get; set; } = default!;

    public DbSet<WorkoutLog> WorkoutLogs { get; set; } = default!;

    public DbSet<Notification> Notifications { get; set; } = default!;

    public DbSet<NutritionPlan> NutritionPlans { get; set; } = default!;

    public DbSet<Meal> Meals { get; set; } = default!;

    public DbSet<ClientNutritionPlan> ClientNutritionPlans { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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
                value => JsonSerializer.Serialize(value, (JsonSerializerOptions?)null),
                value => JsonSerializer.Deserialize<List<string>>(value, (JsonSerializerOptions?)null) ?? new List<string>());
    }
}
