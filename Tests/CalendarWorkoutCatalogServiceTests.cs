using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using Microsoft.Extensions.Logging;
using Moq;
using WebAPI.Services;

namespace Tests;

public sealed class CalendarWorkoutCatalogServiceTests
{
    private const int ClientId = 7;

    private readonly Mock<IWorkoutTemplateRepository> templateRepo = new();
    private readonly Mock<ILogger<CalendarWorkoutCatalogService>> logger = new();

    private CalendarWorkoutCatalogService CreateService() => new(this.templateRepo.Object, this.logger.Object);

    [Fact]
    public async Task GetAvailableWorkoutsAsync_RepositoryReturnsWorkouts_ReturnsMappedDtos()
    {
        var templates = new List<WorkoutTemplate>
        {
            new()
            {
                WorkoutTemplateId = 1,
                Client = new Client { ClientId = ClientId },
                Name = "Push Day",
                Exercises = new List<TemplateExercise>(),
            },
        };

        this.templateRepo
            .Setup(r => r.GetAvailableWorkoutsAsync(ClientId))
            .ReturnsAsync(templates);

        var service = this.CreateService();
        var result = await service.GetAvailableWorkoutsAsync(ClientId, TimeSpan.FromSeconds(5));

        Assert.Single(result.Workouts);
        Assert.Equal("Push Day", result.Workouts[0].Name);
    }

    [Fact]
    public async Task GetAvailableWorkoutsAsync_RepositoryThrows_ReturnsFallbackWorkouts()
    {
        this.templateRepo
            .Setup(r => r.GetAvailableWorkoutsAsync(ClientId))
            .ThrowsAsync(new InvalidOperationException("db error"));

        var service = this.CreateService();
        var result = await service.GetAvailableWorkoutsAsync(ClientId, TimeSpan.FromSeconds(1));

        Assert.Equal(2, result.Workouts.Count);
        Assert.All(result.Workouts, w => Assert.Equal("PREBUILT", w.Type));
    }

    [Fact]
    public void GetFallbackWorkouts_ReturnsFallbackWithExercises()
    {
        var service = this.CreateService();
        var result = service.GetFallbackWorkouts(ClientId);

        Assert.Equal(2, result.Workouts.Count);
        Assert.All(result.Workouts, w =>
        {
            Assert.Equal("PREBUILT", w.Type);
            Assert.Equal(3, w.Exercises.Count);
        });
    }

    [Fact]
    public async Task GetAvailableWorkoutsAsync_Timeout_ReturnsFallbackWorkouts()
    {
        // use a TaskCompletionSource so the "slow" repo never completes on its own —
        // avoids flaky 5-second Task.Delay racing against a 50ms timeout
        var neverCompletes = new TaskCompletionSource<IReadOnlyList<WorkoutTemplate>>();

        this.templateRepo
            .Setup(r => r.GetAvailableWorkoutsAsync(ClientId))
            .Returns(neverCompletes.Task);

        var service = this.CreateService();
        var result = await service.GetAvailableWorkoutsAsync(ClientId, TimeSpan.FromMilliseconds(50));

        Assert.Equal(2, result.Workouts.Count);
        Assert.All(result.Workouts, w => Assert.Equal("PREBUILT", w.Type));
    }

    [Fact]
    public async Task GetAvailableWorkoutsAsync_maps_exercise_details_correctly()
    {
        var templates = new List<WorkoutTemplate>
        {
            new()
            {
                WorkoutTemplateId = 1,
                Client = new Client { ClientId = ClientId },
                Name = "Leg Day",
                Exercises = new List<TemplateExercise>
                {
                    new() { Name = "Squat", TargetSets = 5, TargetReps = 5, TargetWeight = 100, MuscleGroup = MuscleGroup.LEGS },
                    new() { Name = "Leg Press", TargetSets = 4, TargetReps = 8, TargetWeight = 180, MuscleGroup = MuscleGroup.LEGS },
                },
            },
        };

        this.templateRepo
            .Setup(r => r.GetAvailableWorkoutsAsync(ClientId))
            .ReturnsAsync(templates);

        var service = this.CreateService();
        var result = await service.GetAvailableWorkoutsAsync(ClientId, TimeSpan.FromSeconds(5));

        var dto = result.Workouts[0];
        Assert.Equal(2, dto.Exercises.Count);
        Assert.Equal("Squat", dto.Exercises[0].Name);
        Assert.Equal(5, dto.Exercises[0].TargetSets);
        Assert.Equal(100, dto.Exercises[0].TargetWeight);
    }

    [Fact]
    public void FallbackWorkouts_each_have_three_exercises()
    {
        var service = this.CreateService();
        var result = service.GetFallbackWorkouts(ClientId);

        Assert.All(result.Workouts, w =>
        {
            Assert.NotEmpty(w.Name);
            Assert.Equal(3, w.Exercises.Count);
            Assert.All(w.Exercises, e => Assert.True(e.TargetSets > 0));
        });
    }
}
