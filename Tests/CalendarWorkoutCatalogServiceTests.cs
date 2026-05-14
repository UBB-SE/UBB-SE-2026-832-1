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
        this.templateRepo
            .Setup(r => r.GetAvailableWorkoutsAsync(ClientId))
            .Returns(async () =>
            {
                await Task.Delay(5000);
                return new List<WorkoutTemplate>();
            });

        var service = this.CreateService();
        var result = await service.GetAvailableWorkoutsAsync(ClientId, TimeSpan.FromMilliseconds(50));

        Assert.Equal(2, result.Workouts.Count);
    }
}
