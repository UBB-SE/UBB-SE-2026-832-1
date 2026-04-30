using System.Text.Json;
using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

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

    public DbSet<NutritionPlan> NutritionPlans { get; set; } = default!;

    public DbSet<Meal> Meals { get; set; } = default!;

    public DbSet<ClientNutritionPlan> ClientNutritionPlans { get; set; } = default!;

    public DbSet<DailyLog> DailyLogs { get; set; } = default!;

    public DbSet<Conversation> Conversations { get; set; } = default!;

    public DbSet<Message> Messages { get; set; } = default!;

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
                value => JsonSerializer.Serialize(value, (JsonSerializerOptions?)null),
                value => JsonSerializer.Deserialize<List<string>>(value, (JsonSerializerOptions?)null) ?? new List<string>());

        modelBuilder.Entity<DailyLog>(entity =>
        {
            entity.HasOne(dailyLog => dailyLog.User)
                .WithMany()
                .HasForeignKey(dailyLog => dailyLog.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(dailyLog => dailyLog.Meal)
                .WithMany()
                .HasForeignKey(dailyLog => dailyLog.MealId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(dailyLog => dailyLog.LoggedAt).IsRequired();
        });

        modelBuilder.Entity<Ingredient>(entity =>
        {
            entity.Property(ingredient => ingredient.Name).IsRequired().HasMaxLength(200);
        });

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
