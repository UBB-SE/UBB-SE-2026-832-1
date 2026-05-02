using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using Moq;
using WebAPI.Services;

namespace Tests;

public sealed class WorkoutLogServiceTests
{
    private readonly Mock<IWorkoutLogRepository> workoutLogRepository = new();

    private WorkoutLogService CreateService() => new(this.workoutLogRepository.Object);

    [Fact]
    public async Task GetWorkoutHistory_ZeroRating_MapsToNullInDto()
    {
        var logs = new List<WorkoutLog>
        {
            CreateLogWithRating(rating: 0),
        };

        this.workoutLogRepository
            .Setup(r => r.GetWorkoutHistoryAsync(1))
            .ReturnsAsync(logs);

        var service = this.CreateService();
        var result = await service.GetWorkoutHistoryAsync(1);

        Assert.Single(result);
        Assert.Null(result[0].Rating);
    }

    [Fact]
    public async Task GetWorkoutHistory_PositiveRating_PreservedInDto()
    {
        var logs = new List<WorkoutLog>
        {
            CreateLogWithRating(rating: 4.5),
        };

        this.workoutLogRepository
            .Setup(r => r.GetWorkoutHistoryAsync(1))
            .ReturnsAsync(logs);

        var service = this.CreateService();
        var result = await service.GetWorkoutHistoryAsync(1);

        Assert.Equal(4.5, result[0].Rating);
    }

    [Fact]
    public async Task GetWorkoutHistory_NegativeRatingSentinel_MapsToNullInDto()
    {
        var logs = new List<WorkoutLog>
        {
            CreateLogWithRating(rating: -1),
        };

        this.workoutLogRepository
            .Setup(r => r.GetWorkoutHistoryAsync(1))
            .ReturnsAsync(logs);

        var service = this.CreateService();
        var result = await service.GetWorkoutHistoryAsync(1);

        Assert.Null(result[0].Rating);
    }

    [Fact]
    public async Task GetWorkoutHistory_NoLogs_ReturnsEmptyList()
    {
        this.workoutLogRepository
            .Setup(r => r.GetWorkoutHistoryAsync(999))
            .ReturnsAsync(new List<WorkoutLog>());

        var service = this.CreateService();
        var result = await service.GetWorkoutHistoryAsync(999);

        Assert.Empty(result);
    }

    private static WorkoutLog CreateLogWithRating(double rating)
    {
        return new WorkoutLog
        {
            WorkoutLogId = 1,
            Client = new Client { ClientId = 1, Email = "test@test.com", FullName = "Test User" },
            WorkoutName = "Push Day",
            Date = DateTime.Today,
            Duration = TimeSpan.FromMinutes(60),
            Type = WorkoutType.CUSTOM,
            Rating = rating,
            Exercises = new List<LoggedExercise>(),
        };
    }
}
