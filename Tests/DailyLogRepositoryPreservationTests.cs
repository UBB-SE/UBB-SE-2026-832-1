using ClassLibrary.Data;
using ClassLibrary.Models;
using ClassLibrary.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests;

public sealed class DailyLogRepositoryPreservationTests : IDisposable
{
    private readonly AppDbContext context;
    private readonly DailyLogRepository repository;

    public DailyLogRepositoryPreservationTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: $"PreservationTestDb_{Guid.NewGuid()}")
            .Options;

        this.context = new AppDbContext(options);
        this.repository = new DailyLogRepository(this.context);
    }

    [Fact]
    public async Task HasAnyLogsAsync_WithExistingLogs_ReturnsTrue()
    {
        var user = new User
        {
            UserId = 1,
            Username = "testuser",
            Email = "test@example.com",
            FullName = "Test User",
            Password = "password123",
            Role = "User"
        };

        var dailyLog = new DailyLog
        {
            User = user,
            LoggedAt = DateTime.UtcNow,
            Calories = 500,
            Protein = 30,
            Carbohydrates = 60,
            Fats = 15
        };

        this.context.Users.Add(user);
        this.context.DailyLogs.Add(dailyLog);
        await this.context.SaveChangesAsync();

        var result = await this.repository.HasAnyLogsAsync(userId: 1);

        Assert.True(result);
    }

    [Fact]
    public async Task HasAnyLogsAsync_WithNoLogs_ReturnsFalse()
    {
        var user = new User
        {
            UserId = 1,
            Username = "testuser",
            Email = "test@example.com",
            FullName = "Test User",
            Password = "password123",
            Role = "User"
        };

        this.context.Users.Add(user);
        await this.context.SaveChangesAsync();

        var result = await this.repository.HasAnyLogsAsync(userId: 1);

        Assert.False(result);
    }

    [Fact]
    public async Task HasAnyLogsAsync_WithMultipleUsers_ReturnsCorrectResultPerUser()
    {
        var user1 = new User
        {
            UserId = 1,
            Username = "user1",
            Email = "user1@example.com",
            FullName = "User One",
            Password = "password123",
            Role = "User"
        };

        var user2 = new User
        {
            UserId = 2,
            Username = "user2",
            Email = "user2@example.com",
            FullName = "User Two",
            Password = "password123",
            Role = "User"
        };

        var dailyLogForUser1 = new DailyLog
        {
            User = user1,
            LoggedAt = DateTime.UtcNow,
            Calories = 500,
            Protein = 30,
            Carbohydrates = 60,
            Fats = 15
        };

        this.context.Users.AddRange(user1, user2);
        this.context.DailyLogs.Add(dailyLogForUser1);
        await this.context.SaveChangesAsync();

        var user1HasLogs = await this.repository.HasAnyLogsAsync(userId: 1);
        var user2HasLogs = await this.repository.HasAnyLogsAsync(userId: 2);

        Assert.True(user1HasLogs);
        Assert.False(user2HasLogs);
    }

    [Fact]
    public async Task HasFoodItemLoggedTodayAsync_WithFoodItemLoggedToday_ReturnsTrue()
    {
        var user = new User
        {
            UserId = 1,
            Username = "testuser",
            Email = "test@example.com",
            FullName = "Test User",
            Password = "password123",
            Role = "User"
        };

        var foodItem = new FoodItem
        {
            FoodItemId = 1,
            Name = "Apple",
            Calories = 95,
            Protein = 1,
            Carbohydrates = 25,
            Fat = 0
        };

        var dailyLog = new DailyLog
        {
            User = user,
            FoodItem = foodItem,
            LoggedAt = DateTime.UtcNow,
            Calories = 95,
            Protein = 1,
            Carbohydrates = 25,
            Fats = 0
        };

        this.context.Users.Add(user);
        this.context.FoodItems.Add(foodItem);
        this.context.DailyLogs.Add(dailyLog);
        await this.context.SaveChangesAsync();

        var result = await this.repository.HasFoodItemLoggedTodayAsync(userId: 1, foodItemId: 1);

        Assert.True(result);
    }

    [Fact]
    public async Task HasFoodItemLoggedTodayAsync_WithFoodItemNotLoggedToday_ReturnsFalse()
    {
        var user = new User
        {
            UserId = 1,
            Username = "testuser",
            Email = "test@example.com",
            FullName = "Test User",
            Password = "password123",
            Role = "User"
        };

        var foodItem = new FoodItem
        {
            FoodItemId = 1,
            Name = "Apple",
            Calories = 95,
            Protein = 1,
            Carbohydrates = 25,
            Fat = 0
        };

        var dailyLog = new DailyLog
        {
            User = user,
            FoodItem = foodItem,
            LoggedAt = DateTime.UtcNow.AddDays(-1),
            Calories = 95,
            Protein = 1,
            Carbohydrates = 25,
            Fats = 0
        };

        this.context.Users.Add(user);
        this.context.FoodItems.Add(foodItem);
        this.context.DailyLogs.Add(dailyLog);
        await this.context.SaveChangesAsync();

        var result = await this.repository.HasFoodItemLoggedTodayAsync(userId: 1, foodItemId: 1);

        Assert.False(result);
    }

    [Fact]
    public async Task HasFoodItemLoggedTodayAsync_WithDifferentFoodItem_ReturnsFalse()
    {
        var user = new User
        {
            UserId = 1,
            Username = "testuser",
            Email = "test@example.com",
            FullName = "Test User",
            Password = "password123",
            Role = "User"
        };

        var foodItem1 = new FoodItem
        {
            FoodItemId = 1,
            Name = "Apple",
            Calories = 95,
            Protein = 1,
            Carbohydrates = 25,
            Fat = 0
        };

        var foodItem2 = new FoodItem
        {
            FoodItemId = 2,
            Name = "Banana",
            Calories = 105,
            Protein = 1,
            Carbohydrates = 27,
            Fat = 0
        };

        var dailyLog = new DailyLog
        {
            User = user,
            FoodItem = foodItem1,
            LoggedAt = DateTime.UtcNow,
            Calories = 95,
            Protein = 1,
            Carbohydrates = 25,
            Fats = 0
        };

        this.context.Users.Add(user);
        this.context.FoodItems.AddRange(foodItem1, foodItem2);
        this.context.DailyLogs.Add(dailyLog);
        await this.context.SaveChangesAsync();

        var result = await this.repository.HasFoodItemLoggedTodayAsync(userId: 1, foodItemId: 2);

        Assert.False(result);
    }

    [Fact]
    public async Task AddAsync_WithValidLog_AddsLogSuccessfully()
    {
        var user = new User
        {
            UserId = 1,
            Username = "testuser",
            Email = "test@example.com",
            FullName = "Test User",
            Password = "password123",
            Role = "User"
        };

        var foodItem = new FoodItem
        {
            FoodItemId = 1,
            Name = "Apple",
            Calories = 95,
            Protein = 1,
            Carbohydrates = 25,
            Fat = 0
        };

        this.context.Users.Add(user);
        this.context.FoodItems.Add(foodItem);
        await this.context.SaveChangesAsync();

        var dailyLog = new DailyLog
        {
            User = user,
            FoodItem = foodItem,
            LoggedAt = DateTime.UtcNow,
            Calories = 95,
            Protein = 1,
            Carbohydrates = 25,
            Fats = 0
        };

        await this.repository.AddAsync(dailyLog);

        var savedLog = await this.context.DailyLogs.FirstOrDefaultAsync();
        Assert.NotNull(savedLog);
        Assert.Equal(95, savedLog.Calories);
        Assert.Equal(1, savedLog.Protein);
        Assert.Equal(25, savedLog.Carbohydrates);
        Assert.Equal(0, savedLog.Fats);
    }

    [Fact]
    public async Task AddAsync_WithMeal_AddsLogWithMealSuccessfully()
    {
        var user = new User
        {
            UserId = 1,
            Username = "testuser",
            Email = "test@example.com",
            FullName = "Test User",
            Password = "password123",
            Role = "User"
        };

        var nutritionPlan = new NutritionPlan
        {
            NutritionPlanId = 1,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(30)
        };

        var meal = new Meal
        {
            MealId = 1,
            Name = "Breakfast",
            NutritionPlan = nutritionPlan,
            Ingredients = new List<string> { "Eggs", "Toast", "Butter" },
            Instructions = "Cook and serve"
        };

        this.context.Users.Add(user);
        this.context.NutritionPlans.Add(nutritionPlan);
        this.context.Meals.Add(meal);
        await this.context.SaveChangesAsync();

        var dailyLog = new DailyLog
        {
            User = user,
            Meal = meal,
            LoggedAt = DateTime.UtcNow,
            Calories = 500,
            Protein = 30,
            Carbohydrates = 60,
            Fats = 15
        };

        await this.repository.AddAsync(dailyLog);

        var savedLog = await this.context.DailyLogs.FirstOrDefaultAsync();
        Assert.NotNull(savedLog);
        Assert.Equal(500, savedLog.Calories);
    }

    [Fact]
    public async Task AddAsync_WithMultipleLogs_AddsAllLogsSuccessfully()
    {
        var user = new User
        {
            UserId = 1,
            Username = "testuser",
            Email = "test@example.com",
            FullName = "Test User",
            Password = "password123",
            Role = "User"
        };

        this.context.Users.Add(user);
        await this.context.SaveChangesAsync();

        var log1 = new DailyLog
        {
            User = user,
            LoggedAt = DateTime.UtcNow,
            Calories = 500,
            Protein = 30,
            Carbohydrates = 60,
            Fats = 15
        };

        var log2 = new DailyLog
        {
            User = user,
            LoggedAt = DateTime.UtcNow.AddHours(3),
            Calories = 700,
            Protein = 40,
            Carbohydrates = 80,
            Fats = 20
        };

        await this.repository.AddAsync(log1);
        await this.repository.AddAsync(log2);

        var savedLogs = await this.context.DailyLogs.ToListAsync();
        Assert.Equal(2, savedLogs.Count);
        Assert.Equal(500, savedLogs[0].Calories);
        Assert.Equal(700, savedLogs[1].Calories);
    }

    [Fact]
    public async Task GetNutritionTotalsForRangeAsync_WithNoLogsInRange_ReturnsNull()
    {
        var user = new User
        {
            UserId = 1,
            Username = "testuser",
            Email = "test@example.com",
            FullName = "Test User",
            Password = "password123",
            Role = "User"
        };

        this.context.Users.Add(user);
        await this.context.SaveChangesAsync();

        var startDate = DateTime.Today.AddDays(-7);
        var endDate = DateTime.Today.AddDays(-6);

        var result = await this.repository.GetNutritionTotalsForRangeAsync(userId: 1, startDate, endDate);

        Assert.Null(result);
    }

    public void Dispose()
    {
        this.context.Database.EnsureDeleted();
        this.context.Dispose();
    }
}
