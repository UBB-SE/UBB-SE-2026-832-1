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

        modelBuilder.Entity<Favorite>(entity =>
        {
            entity.HasOne(f => f.User)
                .WithMany()
                .HasForeignKey("UserId")
                .IsRequired();

            entity.HasOne(f => f.FoodItem)
                .WithMany()
                .HasForeignKey("FoodItemId")
                .IsRequired();

            entity.HasIndex(f => new
            {
                UserId = EF.Property<int>(f, "UserId"),
                FoodItemId = EF.Property<int>(f, "FoodItemId"),
            }).IsUnique();
        });

        modelBuilder.Entity<FoodItemIngredient>(entity =>
        {
            entity.HasOne(f => f.FoodItem)
                .WithMany()
                .HasForeignKey("FoodItemId")
                .IsRequired();

            entity.HasOne(f => f.Ingredient)
                .WithMany()
                .HasForeignKey("IngredientId")
                .IsRequired();

            entity.HasIndex(f => new
            {
                FoodItemId = EF.Property<int>(f, "FoodItemId"),
                IngredientId = EF.Property<int>(f, "IngredientId"),
            }).IsUnique();
        });

        modelBuilder.Entity<MealPlan>(entity =>
        {
            entity.HasOne(m => m.User)
                .WithMany()
                .HasForeignKey("UserId")
                .IsRequired();
        });

        modelBuilder.Entity<MealPlanFoodItem>(entity =>
        {
            entity.HasOne(m => m.MealPlan)
                .WithMany()
                .HasForeignKey("MealPlanId")
                .IsRequired();

            entity.HasOne(m => m.FoodItem)
                .WithMany()
                .HasForeignKey("FoodItemId")
                .IsRequired();

            entity.HasIndex(m => new
            {
                MealPlanId = EF.Property<int>(m, "MealPlanId"),
                FoodItemId = EF.Property<int>(m, "FoodItemId"),
            }).IsUnique();
        });

        modelBuilder.Entity<ClientNutritionPlan>()
            .HasKey("ClientId", "NutritionPlanId");

        modelBuilder.Entity<ClientNutritionPlan>()
            .HasOne(c => c.Client)
            .WithMany(c => c.ClientNutritionPlans)
            .HasForeignKey("ClientId");

        modelBuilder.Entity<ClientNutritionPlan>()
            .HasOne(c => c.NutritionPlan)
            .WithMany()
            .HasForeignKey("NutritionPlanId");

        modelBuilder.Entity<Meal>()
            .Property(m => m.Ingredients)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>())
            .Metadata.SetValueComparer(new ValueComparer<List<string>>(
                (a, b) => (a ?? new List<string>()).SequenceEqual(b ?? new List<string>()),
                a => a.Aggregate(0, (h, x) => HashCode.Combine(h, x.GetHashCode())),
                a => a.ToList()));

        modelBuilder.Entity<DailyLog>(entity =>
        {
            entity.HasOne(dailyLog => dailyLog.User)
                .WithMany()
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(dailyLog => dailyLog.Meal)
                .WithMany()
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(dailyLog => dailyLog.LoggedAt).IsRequired();
        });

        modelBuilder.Entity<Ingredient>(entity =>
        {
            entity.Property(i => i.Name).IsRequired().HasMaxLength(200);
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasOne(i => i.User)
                .WithMany()
                .HasForeignKey("UserId")
                .IsRequired();

            entity.HasOne(i => i.Ingredient)
                .WithMany()
                .HasForeignKey("IngredientId")
                .IsRequired();
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasOne(n => n.Client)
                .WithMany(c => c.Notifications)
                .HasForeignKey("ClientId")
                .IsRequired();
        });

        modelBuilder.Entity<ShoppingItem>(entity =>
        {
            entity.HasOne(s => s.User)
                .WithMany()
                .HasForeignKey("UserId")
                .IsRequired();

            entity.HasOne(s => s.Ingredient)
                .WithMany()
                .HasForeignKey("IngredientId")
                .IsRequired();
        });

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

        modelBuilder.Entity<TemplateExercise>(entity =>
        {
            entity.HasOne(t => t.WorkoutTemplate)
                .WithMany(w => w.Exercises)
                .HasForeignKey("WorkoutTemplateId")
                .IsRequired();
        });

        modelBuilder.Entity<UserData>(entity =>
        {
            entity.HasOne(u => u.User)
                .WithMany()
                .HasForeignKey("UserId")
                .IsRequired();
        });

        modelBuilder.Entity<WorkoutLog>(entity =>
        {
            entity.HasOne(w => w.Client)
                .WithMany(c => c.WorkoutLogs)
                .HasForeignKey("ClientId")
                .IsRequired();
        });

        modelBuilder.Entity<WorkoutTemplate>(entity =>
        {
            entity.HasOne(w => w.Client)
                .WithMany()
                .HasForeignKey("ClientId")
                .IsRequired();
        });


        modelBuilder.Entity<Reminder>(entity =>
        {
            entity.Property(reminder => reminder.Name)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(reminder => reminder.Frequency)
                .HasMaxLength(50);

            entity.HasOne(reminder => reminder.User)
                .WithMany(user => user.Reminders)
                .IsRequired();
        });
    }
}