using ClassLibrary.DTOs;
using ClassLibrary.DTOs.Analytics;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using Microsoft.Extensions.Configuration;
using Moq;
using WebAPI.IServices;
using WebAPI.Services;

namespace Tests;

public sealed class ClientServiceTests
{
    private readonly Mock<IAchievementsRepository> achievementsRepo = new();
    private readonly Mock<INotificationRepository> notificationRepo = new();
    private readonly Mock<INutritionRepository> nutritionRepo = new();
    private readonly Mock<IAnalyticsService> analyticsService = new();
    private readonly Mock<IWorkoutLogRepository> workoutLogRepo = new();
    private readonly Mock<IWorkoutTemplateRepository> templateRepo = new();
    private readonly Mock<IClientRepository> clientRepo = new();
    private readonly Mock<IHttpClientFactory> httpClientFactory = new();
    private readonly Mock<IConfiguration> configuration = new();

    private ClientService CreateService() => new(
        this.achievementsRepo.Object,
        this.notificationRepo.Object,
        this.nutritionRepo.Object,
        this.analyticsService.Object,
        this.workoutLogRepo.Object,
        this.templateRepo.Object,
        this.clientRepo.Object,
        this.httpClientFactory.Object,
        this.configuration.Object);

    [Fact]
    public async Task GetWorkoutHistoryAsync_ValidClient_ReturnsWorkoutLogs()
    {
        var logs = new List<WorkoutLog>
        {
            new()
            {
                WorkoutLogId = 1,
                WorkoutName = "Leg Day",
                Client = new Client { ClientId = 1 },
                Exercises = new List<LoggedExercise>(),
            },
        };

        this.workoutLogRepo
            .Setup(r => r.GetWorkoutHistoryAsync(1))
            .ReturnsAsync(logs);

        var service = this.CreateService();
        var result = await service.GetWorkoutHistoryAsync(1);

        Assert.Single(result);
        Assert.Equal("Leg Day", result[0].WorkoutName);
    }

    [Fact]
    public async Task GetAchievementsAsync_ReturnsMappedDtos()
    {
        var showcase = new List<AchievementShowcaseItem>
        {
            new() { AchievementId = 1, Title = "3-Day Streak", IsUnlocked = true },
            new() { AchievementId = 2, Title = "Iron Week", IsUnlocked = false },
        };

        this.achievementsRepo
            .Setup(r => r.GetAchievementShowcaseForClientAsync(1))
            .ReturnsAsync(showcase);

        var service = this.CreateService();
        var result = await service.GetAchievementsAsync(1);

        Assert.Equal(2, result.Count);
        Assert.True(result[0].IsUnlocked);
        Assert.False(result[1].IsUnlocked);
    }

    [Fact]
    public async Task GetActiveNutritionPlanAsync_InvalidClientId_ReturnsNull()
    {
        var service = this.CreateService();
        var result = await service.GetActiveNutritionPlanAsync(0);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetActiveNutritionPlanAsync_NoActivePlan_ReturnsNull()
    {
        var expiredPlan = new NutritionPlan
        {
            NutritionPlanId = 1,
            StartDate = DateTime.Today.AddDays(-30),
            EndDate = DateTime.Today.AddDays(-1),
            Meals = new List<Meal>(),
        };

        this.nutritionRepo
            .Setup(r => r.GetNutritionPlansForClientAsync(1))
            .ReturnsAsync(new List<NutritionPlan> { expiredPlan });

        var service = this.CreateService();
        var result = await service.GetActiveNutritionPlanAsync(1);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetActiveNutritionPlanAsync_ActivePlanExists_ReturnsMappedDto()
    {
        var plan = new NutritionPlan
        {
            NutritionPlanId = 1,
            StartDate = DateTime.Today.AddDays(-1),
            EndDate = DateTime.Today.AddDays(7),
            Meals = new List<Meal>
            {
                new() { MealId = 1, Name = "Oatmeal", Ingredients = new List<string> { "Oats" }, Instructions = "Cook" },
            },
        };

        this.nutritionRepo
            .Setup(r => r.GetNutritionPlansForClientAsync(1))
            .ReturnsAsync(new List<NutritionPlan> { plan });

        var service = this.CreateService();
        var result = await service.GetActiveNutritionPlanAsync(1);

        Assert.NotNull(result);
        Assert.Single(result.Meals);
        Assert.Equal("Oatmeal", result.Meals[0].Name);
    }

    [Fact]
    public async Task FinalizeWorkoutAsync_NullWorkoutLog_ReturnsFalse()
    {
        var request = new FinalizeWorkoutRequestDataTransferObject { WorkoutLog = null! };

        var service = this.CreateService();
        var result = await service.FinalizeWorkoutAsync(request);

        Assert.False(result);
    }

    [Fact]
    public async Task FinalizeWorkoutAsync_InvalidClientId_ReturnsFalse()
    {
        var request = new FinalizeWorkoutRequestDataTransferObject
        {
            WorkoutLog = new WorkoutLogDataTransferObject
            {
                Client = new ClientDataTransferObject { ClientId = 0 },
            },
        };

        var service = this.CreateService();
        var result = await service.FinalizeWorkoutAsync(request);

        Assert.False(result);
    }

    [Fact]
    public async Task FinalizeWorkoutAsync_ClientNotFound_ReturnsFalse()
    {
        var request = new FinalizeWorkoutRequestDataTransferObject
        {
            WorkoutLog = new WorkoutLogDataTransferObject
            {
                Client = new ClientDataTransferObject { ClientId = 1 },
                WorkoutName = "Leg Day",
            },
        };

        this.clientRepo
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync((Client?)null);

        var service = this.CreateService();
        var result = await service.FinalizeWorkoutAsync(request);

        Assert.False(result);
    }

    [Fact]
    public async Task FinalizeWorkoutAsync_ValidRequest_SavesAndReturnsTrue()
    {
        var request = new FinalizeWorkoutRequestDataTransferObject
        {
            WorkoutLog = new WorkoutLogDataTransferObject
            {
                Client = new ClientDataTransferObject { ClientId = 1 },
                WorkoutName = "Upper Body",
                Exercises = new List<LoggedExerciseDataTransferObject>(),
            },
        };

        this.clientRepo
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Client { ClientId = 1 });

        var service = this.CreateService();
        var result = await service.FinalizeWorkoutAsync(request);

        Assert.True(result);
        this.workoutLogRepo.Verify(r => r.SaveWorkoutLogAsync(It.IsAny<WorkoutLog>()), Times.Once);
    }

    [Fact]
    public async Task ModifyWorkoutAsync_NullInput_ReturnsFalse()
    {
        var service = this.CreateService();
        var result = await service.ModifyWorkoutAsync(null!);

        Assert.False(result);
    }

    [Fact]
    public async Task ModifyWorkoutAsync_InvalidWorkoutLogId_ReturnsFalse()
    {
        var dto = new WorkoutLogDataTransferObject
        {
            WorkoutLogId = 0,
            Client = new ClientDataTransferObject { ClientId = 1 },
        };

        var service = this.CreateService();
        var result = await service.ModifyWorkoutAsync(dto);

        Assert.False(result);
    }

    [Fact]
    public async Task ModifyWorkoutAsync_ValidRequest_UpdatesAndReturnsTrue()
    {
        var dto = new WorkoutLogDataTransferObject
        {
            WorkoutLogId = 5,
            Client = new ClientDataTransferObject { ClientId = 1 },
            WorkoutName = "Pull Day",
            Exercises = new List<LoggedExerciseDataTransferObject>(),
        };

        this.clientRepo
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Client { ClientId = 1 });

        var service = this.CreateService();
        var result = await service.ModifyWorkoutAsync(dto);

        Assert.True(result);
        this.workoutLogRepo.Verify(r => r.UpdateWorkoutLogAsync(It.IsAny<WorkoutLog>()), Times.Once);
    }

    [Fact]
    public async Task GetAvailableWorkoutsAsync_ReturnsTemplatesAsDtos()
    {
        var templates = new List<WorkoutTemplate>
        {
            new()
            {
                WorkoutTemplateId = 1,
                Name = "Full Body",
                Client = new Client { ClientId = 1 },
                Exercises = new List<TemplateExercise>
                {
                    new() { Name = "Squat", MuscleGroup = MuscleGroup.LEGS, TargetSets = 3, TargetReps = 10, TargetWeight = 60 },
                },
            },
        };

        this.templateRepo
            .Setup(r => r.GetAvailableWorkoutsAsync(1))
            .ReturnsAsync(templates);

        var service = this.CreateService();
        var result = await service.GetAvailableWorkoutsAsync(1);

        Assert.Single(result);
        Assert.Equal("Full Body", result[0].Name);
        Assert.Single(result[0].Exercises);
    }

    [Fact]
    public async Task GetClientProfileSnapshotAsync_WithWorkouts_ReturnsPopulatedSnapshot()
    {
        var logs = new List<WorkoutLog>
        {
            new()
            {
                WorkoutLogId = 1,
                WorkoutName = "Leg Day",
                Date = DateTime.Now,
                TotalCaloriesBurned = 300,
                Client = new Client { ClientId = 1 },
                Exercises = new List<LoggedExercise>
                {
                    new()
                    {
                        ExerciseName = "Squat",
                        Sets = new List<LoggedSet>(),
                    },
                },
            },
        };

        this.workoutLogRepo
            .Setup(r => r.GetWorkoutHistoryAsync(1))
            .ReturnsAsync(logs);

        this.nutritionRepo
            .Setup(r => r.GetNutritionPlansForClientAsync(1))
            .ReturnsAsync(new List<NutritionPlan>());

        var service = this.CreateService();
        var result = await service.GetClientProfileSnapshotAsync(1);

        Assert.Contains("300", result.CaloriesSummary);
        Assert.Contains("Leg Day", result.LatestSessionHint);
        Assert.Single(result.LoggedExercises);
    }

    [Fact]
    public async Task GetDashboardSummaryAsync_DelegatesToAnalyticsService()
    {
        var expected = new DashboardSummary { TotalWorkouts = 10 };
        this.analyticsService
            .Setup(s => s.GetDashboardSummaryAsync(1))
            .ReturnsAsync(expected);

        var service = this.CreateService();
        var result = await service.GetDashboardSummaryAsync(1);

        Assert.Equal(10, result.TotalWorkouts);
    }

    [Fact]
    public async Task SyncNutritionAsync_NullRequest_ReturnsFalse()
    {
        var service = this.CreateService();
        var result = await service.SyncNutritionAsync(null!);

        Assert.False(result);
    }

    [Fact]
    public async Task SyncNutritionAsync_InvalidClientId_ReturnsFalse()
    {
        var request = new NutritionSyncRequestDataTransferObject { ClientId = 0 };

        var service = this.CreateService();
        var result = await service.SyncNutritionAsync(request);

        Assert.False(result);
    }
}
