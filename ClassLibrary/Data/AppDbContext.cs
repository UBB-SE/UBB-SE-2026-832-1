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
            entity.HasOne(favorite => favorite.User)
                .WithMany()
                .HasForeignKey("UserId")
                .IsRequired();

            entity.HasOne(favorite => favorite.FoodItem)
                .WithMany()
                .HasForeignKey("FoodItemId")
                .IsRequired();

            entity.HasIndex(favorite => new
            {
                UserId = EF.Property<int>(favorite, "UserId"),
                FoodItemId = EF.Property<int>(favorite, "FoodItemId"),
            }).IsUnique();
        });

        modelBuilder.Entity<FoodItemIngredient>(entity =>
        {
            entity.HasOne(foodItemIngredient => foodItemIngredient.FoodItem)
                .WithMany()
                .HasForeignKey("FoodItemId")
                .IsRequired();

            entity.HasOne(foodItemIngredient => foodItemIngredient.Ingredient)
                .WithMany()
                .HasForeignKey("IngredientId")
                .IsRequired();

            entity.HasIndex(foodItemIngredient => new
            {
                FoodItemId = EF.Property<int>(foodItemIngredient, "FoodItemId"),
                IngredientId = EF.Property<int>(foodItemIngredient, "IngredientId"),
            }).IsUnique();
        });

        modelBuilder.Entity<MealPlan>(entity =>
        {
            entity.HasOne(mealPlan => mealPlan.User)
                .WithMany()
                .HasForeignKey("UserId")
                .IsRequired();
        });

        modelBuilder.Entity<MealPlanFoodItem>(entity =>
        {
            entity.HasOne(mealPlanFoodItem => mealPlanFoodItem.MealPlan)
                .WithMany()
                .HasForeignKey("MealPlanId")
                .IsRequired();

            entity.HasOne(mealPlanFoodItem => mealPlanFoodItem.FoodItem)
                .WithMany()
                .HasForeignKey("FoodItemId")
                .IsRequired();

            entity.HasIndex(mealPlanFoodItem => new
            {
                MealPlanId = EF.Property<int>(mealPlanFoodItem, "MealPlanId"),
                FoodItemId = EF.Property<int>(mealPlanFoodItem, "FoodItemId"),
            }).IsUnique();
        });

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

        // ✅ TRECHO CORRIGIDO (EXATAMENTE O QUE O REVIEWER PEDIU)
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
            entity.Property(ingredient => ingredient.Name)
                .IsRequired()
                .HasMaxLength(200);
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasOne(inventory => inventory.User)
                .WithMany()
                .HasForeignKey("UserId")
                .IsRequired();

            entity.HasOne(inventory => inventory.Ingredient)
                .WithMany()
                .HasForeignKey("IngredientId")
                .IsRequired();
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasOne(notification => notification.Client)
                .WithMany(client => client.Notifications)
                .HasForeignKey("ClientId")
                .IsRequired();
        });

        modelBuilder.Entity<ShoppingItem>(entity =>
        {
            entity.HasOne(shoppingItem => shoppingItem.User)
                .WithMany()
                .HasForeignKey("UserId")
                .IsRequired();

            entity.HasOne(shoppingItem => shoppingItem.Ingredient)
                .WithMany()
                .HasForeignKey("IngredientId")
                .IsRequired();
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

        modelBuilder.Entity<TemplateExercise>(entity =>
        {
            entity.HasOne(templateExercise => templateExercise.WorkoutTemplate)
                .WithMany(workoutTemplate => workoutTemplate.Exercises)
                .HasForeignKey("WorkoutTemplateId")
                .IsRequired();
        });

        modelBuilder.Entity<UserData>(entity =>
        {
            entity.HasOne(userData => userData.User)
                .WithMany()
                .HasForeignKey("UserId")
                .IsRequired();
        });

        modelBuilder.Entity<WorkoutLog>(entity =>
        {
            entity.HasOne(workoutLog => workoutLog.Client)
                .WithMany(client => client.WorkoutLogs)
                .HasForeignKey("ClientId")
                .IsRequired();
        });

        modelBuilder.Entity<WorkoutTemplate>(entity =>
        {
            entity.HasOne(workoutTemplate => workoutTemplate.Client)
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