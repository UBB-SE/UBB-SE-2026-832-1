using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using Moq;
using WebAPI.Services;

namespace Tests;

public sealed class AnalyticsServiceTests
{
    private readonly Mock<IWorkoutAnalyticsRepository> analyticsRepo = new();
    private readonly Mock<IWorkoutLogRepository> workoutLogRepo = new();

    private AnalyticsService CreateService() => new(this.analyticsRepo.Object, this.workoutLogRepo.Object);

    [Theory]
    [InlineData(2.5f, "light")]
    [InlineData(0.0f, "light")]
    [InlineData(2.9f, "light")]
    public void DetermineIntensityTag_BelowThree_ReturnsLight(float met, string expected)
    {
        var service = this.CreateService();
        Assert.Equal(expected, service.DetermineIntensityTag(met));
    }

    [Theory]
    [InlineData(3.0f, "moderate")]
    [InlineData(4.0f, "moderate")]
    [InlineData(5.9f, "moderate")]
    public void DetermineIntensityTag_ThreeToSix_ReturnsModerate(float met, string expected)
    {
        var service = this.CreateService();
        Assert.Equal(expected, service.DetermineIntensityTag(met));
    }

    [Theory]
    [InlineData(6.0f, "intense")]
    [InlineData(7.0f, "intense")]
    [InlineData(10.0f, "intense")]
    public void DetermineIntensityTag_SixOrAbove_ReturnsIntense(float met, string expected)
    {
        var service = this.CreateService();
        Assert.Equal(expected, service.DetermineIntensityTag(met));
    }

    [Fact]
    public void DetermineIntensityTag_ExactBoundaryAtThree_IsModerateNotLight()
    {
        var service = this.CreateService();
        Assert.Equal("moderate", service.DetermineIntensityTag(3.0f));
    }

    [Fact]
    public void DetermineIntensityTag_ExactBoundaryAtSix_IsIntenseNotModerate()
    {
        var service = this.CreateService();
        Assert.Equal("intense", service.DetermineIntensityTag(6.0f));
    }

    [Fact]
    public async Task GetConsistencyLastFourWeeks_ReturnsExactlyFourBuckets()
    {
        this.analyticsRepo
            .Setup(repository => repository.GetWorkoutsInRangeAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(new List<WorkoutLog>());

        var service = this.CreateService();
        var buckets = await service.GetConsistencyLastFourWeeksAsync(clientId: 1);

        Assert.Equal(4, buckets.Count);
    }

    [Fact]
    public async Task GetConsistencyLastFourWeeks_AllBucketsStartOnMonday()
    {
        this.analyticsRepo
            .Setup(repository => repository.GetWorkoutsInRangeAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(new List<WorkoutLog>());

        var service = this.CreateService();
        var buckets = await service.GetConsistencyLastFourWeeksAsync(clientId: 1);

        foreach (var bucket in buckets)
        {
            Assert.Equal(DayOfWeek.Monday, bucket.WeekStart.DayOfWeek);
        }
    }

    [Fact]
    public async Task GetConsistencyLastFourWeeks_BucketsAreConsecutiveWeeks()
    {
        this.analyticsRepo
            .Setup(repository => repository.GetWorkoutsInRangeAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(new List<WorkoutLog>());

        var service = this.CreateService();
        var buckets = await service.GetConsistencyLastFourWeeksAsync(clientId: 1);

        for (int i = 1; i < buckets.Count; i++)
        {
            int daysBetween = buckets[i].WeekStart.DayNumber - buckets[i - 1].WeekStart.DayNumber;
            Assert.Equal(7, daysBetween);
        }
    }

    [Fact]
    public void GetMondayOfWeek_Wednesday_ReturnsPrecedingMonday()
    {
        var wednesday = new DateOnly(2026, 5, 6);
        var monday = AnalyticsService.GetMondayOfWeek(wednesday);

        Assert.Equal(new DateOnly(2026, 5, 4), monday);
        Assert.Equal(DayOfWeek.Monday, monday.DayOfWeek);
    }

    [Fact]
    public void GetMondayOfWeek_Sunday_ReturnsPrecedingMonday()
    {
        var sunday = new DateOnly(2026, 5, 3);
        var monday = AnalyticsService.GetMondayOfWeek(sunday);

        Assert.Equal(new DateOnly(2026, 4, 27), monday);
    }

    [Fact]
    public void GetMondayOfWeek_Monday_ReturnsSameDay()
    {
        var monday = new DateOnly(2026, 5, 4);
        var result = AnalyticsService.GetMondayOfWeek(monday);

        Assert.Equal(monday, result);
    }

    [Fact]
    public void ComputeCaloriesForWorkout_PopulatesExerciseCaloriesAndTotals()
    {
        var log = new WorkoutLog
        {
            Duration = TimeSpan.FromHours(1),
            Exercises = new List<LoggedExercise>
            {
                new() { ExerciseName = "Bench Press", MetabolicEquivalent = 0, Sets = new List<LoggedSet>() },
                new() { ExerciseName = "Deadlift", MetabolicEquivalent = 0, Sets = new List<LoggedSet>() },
            },
        };

        var service = this.CreateService();
        service.ComputeCaloriesForWorkout(log, clientWeightKg: 80);

        Assert.True(log.TotalCaloriesBurned > 0);
        Assert.True(log.AverageMetabolicEquivalent > 0);
        Assert.False(string.IsNullOrEmpty(log.IntensityTag));
        Assert.All(log.Exercises, exercise => Assert.True(exercise.ExerciseCaloriesBurned > 0));
    }

    [Fact]
    public void ComputeCaloriesForWorkout_NoExercises_DoesNothing()
    {
        var log = new WorkoutLog
        {
            Duration = TimeSpan.FromHours(1),
            Exercises = new List<LoggedExercise>(),
        };

        var service = this.CreateService();
        service.ComputeCaloriesForWorkout(log, clientWeightKg: 80);

        Assert.Equal(0, log.TotalCaloriesBurned);
    }
}
