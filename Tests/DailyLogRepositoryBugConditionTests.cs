using ClassLibrary.Data;
using ClassLibrary.Models;
using ClassLibrary.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests;

public sealed class DailyLogRepositoryBugConditionTests : IDisposable
{
    private readonly AppDbContext context;
    private readonly DailyLogRepository dailyLogRepository;
    private readonly WorkoutLogRepository workoutLogRepository;

    public DailyLogRepositoryBugConditionTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: $"BugConditionTestDb_{Guid.NewGuid()}")
            .Options;

        this.context = new AppDbContext(options);
        this.dailyLogRepository = new DailyLogRepository(this.context);
        this.workoutLogRepository = new WorkoutLogRepository(this.context);

        this.SeedTestData();
    }

    private void SeedTestData()
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

        var client = new Client
        {
            ClientId = 1,
            User = user,
            Email = "test@example.com",
            FullName = "Test User",
            Weight = 75.0,
            Height = 180.0,
            PrimaryGoal = "Maintain"
        };

        var dailyLog1 = new DailyLog
        {
            DailyLogId = 1,
            User = user,
            LoggedAt = DateTime.Today,
            Calories = 500,
            Protein = 30,
            Carbohydrates = 60,
            Fats = 15
        };

        var dailyLog2 = new DailyLog
        {
            DailyLogId = 2,
            User = user,
            LoggedAt = DateTime.Today.AddHours(6),
            Calories = 700,
            Protein = 40,
            Carbohydrates = 80,
            Fats = 20
        };

        var workoutLog1 = new WorkoutLog
        {
            WorkoutLogId = 1,
            Client = client,
            WorkoutName = "Morning Run",
            Date = DateTime.Today,
            Duration = TimeSpan.FromMinutes(30),
            Type = WorkoutType.CUSTOM,
            TotalCaloriesBurned = 300,
            AverageMetabolicEquivalent = 8.0f,
            IntensityTag = "Moderate",
            Rating = 4.5,
            TrainerNotes = "Good pace"
        };

        var workoutLog2 = new WorkoutLog
        {
            WorkoutLogId = 2,
            Client = client,
            WorkoutName = "Evening Weights",
            Date = DateTime.Today,
            Duration = TimeSpan.FromMinutes(45),
            Type = WorkoutType.PREBUILT,
            TotalCaloriesBurned = 250,
            AverageMetabolicEquivalent = 6.0f,
            IntensityTag = "High",
            Rating = 5.0,
            TrainerNotes = "Great form"
        };

        this.context.Users.Add(user);
        this.context.Clients.Add(client);
        this.context.DailyLogs.AddRange(dailyLog1, dailyLog2);
        this.context.WorkoutLogs.AddRange(workoutLog1, workoutLog2);
        this.context.SaveChanges();
    }

    [Fact]
    public async Task GetNutritionTotalsForRangeAsync_WithValidUserIdAndData_ShouldExecuteSuccessfully()
    {
        int userId = 1;
        DateTime startDate = DateTime.Today;
        DateTime endDate = DateTime.Today.AddDays(1);

        var result = await this.dailyLogRepository.GetNutritionTotalsForRangeAsync(userId, startDate, endDate);

        Assert.NotNull(result);
        Assert.Equal(1200, result.Calories);
        Assert.Equal(70, result.Protein);
        Assert.Equal(140, result.Carbohydrates);
        Assert.Equal(35, result.Fats);
    }

    [Fact]
    public async Task GetTotalCaloriesBurnedForRangeAsync_WithValidUserIdAndData_ShouldExecuteSuccessfully()
    {
        int userId = 1;
        DateTime startDate = DateTime.Today;
        DateTime endDate = DateTime.Today.AddDays(1);

        var result = await this.workoutLogRepository.GetTotalCaloriesBurnedForRangeAsync(userId, startDate, endDate);

        Assert.Equal(550.0, result);
    }

    [Fact]
    public async Task GetNutritionTotalsForRangeAsync_WithNoData_ShouldReturnNull()
    {
        int userId = 1;
        DateTime startDate = DateTime.Today.AddDays(-7);
        DateTime endDate = DateTime.Today.AddDays(-6);

        var result = await this.dailyLogRepository.GetNutritionTotalsForRangeAsync(userId, startDate, endDate);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetTotalCaloriesBurnedForRangeAsync_WithNoData_ShouldReturnZero()
    {
        int userId = 1;
        DateTime startDate = DateTime.Today.AddDays(-7);
        DateTime endDate = DateTime.Today.AddDays(-6);

        var result = await this.workoutLogRepository.GetTotalCaloriesBurnedForRangeAsync(userId, startDate, endDate);

        Assert.Equal(0.0, result);
    }

    public void Dispose()
    {
        this.context.Database.EnsureDeleted();
        this.context.Dispose();
    }
}
