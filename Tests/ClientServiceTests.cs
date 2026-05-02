using ClassLibrary.DTOs;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using Microsoft.Extensions.Configuration;
using Moq;
using WebAPI.Services;

namespace Tests;

public sealed class ClientServiceTests
{
    private readonly Mock<IAchievementsRepository> achievementsRepo = new();
    private readonly Mock<INotificationRepository> notificationRepo = new();
    private readonly Mock<INutritionRepository> nutritionRepo = new();
    private readonly Mock<IWorkoutLogRepository> workoutLogRepo = new();
    private readonly Mock<IWorkoutTemplateRepository> workoutTemplateRepo = new();
    private readonly Mock<IClientRepository> clientRepo = new();
    private readonly Mock<IHttpClientFactory> httpClientFactory = new();
    private readonly Mock<IConfiguration> configuration = new();

    private ClientService CreateService() => new(
        this.achievementsRepo.Object,
        this.notificationRepo.Object,
        this.nutritionRepo.Object,
        this.workoutLogRepo.Object,
        this.workoutTemplateRepo.Object,
        this.clientRepo.Object,
        this.httpClientFactory.Object,
        this.configuration.Object);

    [Fact]
    public async Task ModifyWorkout_WithValidLog_DelegatesToRepository()
    {
        var client = new Client { ClientId = 5, Email = "a@b.com", FullName = "Alice" };
        this.clientRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(client);

        var dto = new WorkoutLogDataTransferObject
        {
            WorkoutLogId = 10,
            Client = new ClientDataTransferObject { ClientId = 5 },
            WorkoutName = "Updated Push Day",
            Date = DateTime.Today,
            Duration = TimeSpan.FromMinutes(45),
            Type = "CUSTOM",
            Exercises = new List<LoggedExerciseDataTransferObject>(),
        };

        var service = this.CreateService();
        bool result = await service.ModifyWorkoutAsync(dto);

        Assert.True(result);
        this.workoutLogRepo.Verify(r => r.UpdateWorkoutLogAsync(It.Is<WorkoutLog>(
            log => log.WorkoutLogId == 10 && log.WorkoutName == "Updated Push Day")), Times.Once);
    }

    [Fact]
    public async Task ModifyWorkout_WithNullDto_ReturnsFalse()
    {
        var service = this.CreateService();
        bool result = await service.ModifyWorkoutAsync(null!);

        Assert.False(result);
        this.workoutLogRepo.Verify(r => r.UpdateWorkoutLogAsync(It.IsAny<WorkoutLog>()), Times.Never);
    }

    [Fact]
    public async Task ModifyWorkout_WithZeroWorkoutLogId_ReturnsFalse()
    {
        var dto = new WorkoutLogDataTransferObject
        {
            WorkoutLogId = 0,
            Client = new ClientDataTransferObject { ClientId = 5 },
        };

        var service = this.CreateService();
        bool result = await service.ModifyWorkoutAsync(dto);

        Assert.False(result);
    }

    [Fact]
    public async Task ModifyWorkout_WhenClientNotFound_ReturnsFalse()
    {
        this.clientRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Client?)null);

        var dto = new WorkoutLogDataTransferObject
        {
            WorkoutLogId = 10,
            Client = new ClientDataTransferObject { ClientId = 999 },
            WorkoutName = "Ghost Workout",
            Exercises = new List<LoggedExerciseDataTransferObject>(),
        };

        var service = this.CreateService();
        bool result = await service.ModifyWorkoutAsync(dto);

        Assert.False(result);
        this.workoutLogRepo.Verify(r => r.UpdateWorkoutLogAsync(It.IsAny<WorkoutLog>()), Times.Never);
    }

    [Fact]
    public async Task FinalizeWorkout_WithNullRequest_ReturnsFalse()
    {
        var service = this.CreateService();
        bool result = await service.FinalizeWorkoutAsync(null!);

        Assert.False(result);
    }

    [Fact]
    public async Task FinalizeWorkout_WithValidRequest_SavesAndReturnsTrue()
    {
        var client = new Client { ClientId = 3, Email = "c@d.com", FullName = "Charlie" };
        this.clientRepo.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(client);

        var request = new FinalizeWorkoutRequestDataTransferObject
        {
            WorkoutLog = new WorkoutLogDataTransferObject
            {
                Client = new ClientDataTransferObject { ClientId = 3 },
                WorkoutName = "Leg Day",
                Duration = TimeSpan.FromMinutes(50),
                Type = "CUSTOM",
                Exercises = new List<LoggedExerciseDataTransferObject>(),
            },
        };

        var service = this.CreateService();
        bool result = await service.FinalizeWorkoutAsync(request);

        Assert.True(result);
        this.workoutLogRepo.Verify(r => r.SaveWorkoutLogAsync(It.Is<WorkoutLog>(
            log => log.WorkoutName == "Leg Day")), Times.Once);
    }
}
