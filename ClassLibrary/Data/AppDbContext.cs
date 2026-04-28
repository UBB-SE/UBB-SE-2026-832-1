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

    public DbSet<DailyLog> DailyLogs { get; set; } = default!;

    public DbSet<Ingredient> Ingredients { get; set; } = default!;
    public DbSet<Conversation> Conversations { get; set; } = default!;

    public DbSet<Message> Messages { get; set; } = default!;

    public DbSet<ShoppingItem> ShoppingItems { get; set; } = default!;

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

        // DailyLog configuration - relationships with explicit ClassNameId foreign keys
        modelBuilder.Entity<DailyLog>(entity =>
        {
            entity.HasKey(dl => dl.Id);

            entity.HasOne(dl => dl.User)
                .WithMany()
                .HasForeignKey(dl => dl.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(dl => dl.Meal)
                .WithMany()
                .HasForeignKey(dl => dl.MealId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(dl => dl.LoggedAt).IsRequired();
        });

        // Ingredient configuration
        modelBuilder.Entity<Ingredient>(entity =>
        {
            entity.HasKey(i => i.FoodId);
            entity.Property(i => i.Name).IsRequired().HasMaxLength(200);
        });
=========
        modelBuilder.Entity<Conversation>()
            .HasOne(conversation => conversation.User)
            .WithMany()
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Conversation>()
            .HasMany(conversation => conversation.Messages)
            .WithOne(message => message.Conversation)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Message>()
            .HasOne(message => message.Sender)
            .WithMany()
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}
