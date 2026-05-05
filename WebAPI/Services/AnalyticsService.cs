using ClassLibrary.DTOs.Analytics;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using ClassLibrary.Services;
using WebAPI.IServices;

namespace WebAPI.Services;

public sealed class AnalyticsService : IAnalyticsService
{
    private const float LIGHT_THRESHOLD = 3.0f;
    private const float MODERATE_THRESHOLD = 6.0f;
    private const string INTENSITY_LIGHT = "light";
    private const string INTENSITY_MODERATE = "moderate";
    private const string INTENSITY_INTENSE = "intense";
    private const int CONSISTENCY_WEEKS = 4;
    private const int DAYS_PER_WEEK = 7;
    private const int SEVEN_DAYS = 7;

    private readonly IWorkoutAnalyticsRepository analyticsRepository;
    private readonly IWorkoutLogRepository workoutLogRepository;

    public AnalyticsService(
        IWorkoutAnalyticsRepository analyticsRepository,
        IWorkoutLogRepository workoutLogRepository)
    {
        this.analyticsRepository = analyticsRepository;
        this.workoutLogRepository = workoutLogRepository;
    }

    public void ComputeCaloriesForWorkout(WorkoutLog log, double clientWeightKg)
    {
        if (log.Exercises.Count == 0 || log.Duration == TimeSpan.Zero)
        {
            return;
        }

        TimeSpan durationPerExercise = log.Duration / log.Exercises.Count;

        foreach (var exercise in log.Exercises)
        {
            if (exercise.MetabolicEquivalent <= 0)
            {
                exercise.MetabolicEquivalent = (float)ExerciseCalorieCalculator.GetMetabolicEquivalent(exercise.ExerciseName);
            }

            exercise.ExerciseCaloriesBurned = ExerciseCalorieCalculator.CalculateCalories(
                exercise.MetabolicEquivalent, clientWeightKg, durationPerExercise);
        }

        log.TotalCaloriesBurned = log.Exercises.Sum(exercise => exercise.ExerciseCaloriesBurned);
        log.AverageMetabolicEquivalent = log.Exercises.Average(exercise => exercise.MetabolicEquivalent);
        log.IntensityTag = DetermineIntensityTag(log.AverageMetabolicEquivalent);
    }

    public string DetermineIntensityTag(float averageMetabolicEquivalent)
    {
        if (averageMetabolicEquivalent < LIGHT_THRESHOLD)
        {
            return INTENSITY_LIGHT;
        }

        if (averageMetabolicEquivalent < MODERATE_THRESHOLD)
        {
            return INTENSITY_MODERATE;
        }

        return INTENSITY_INTENSE;
    }

    public async Task<IReadOnlyList<ConsistencyWeekBucket>> GetConsistencyLastFourWeeksAsync(int clientId)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var mondayThisWeek = GetMondayOfWeek(today);
        var buckets = new List<ConsistencyWeekBucket>(CONSISTENCY_WEEKS);

        for (int i = 0; i < CONSISTENCY_WEEKS; i++)
        {
            var weekStart = mondayThisWeek.AddDays(-(CONSISTENCY_WEEKS - 1 - i) * DAYS_PER_WEEK);
            var weekEnd = weekStart.AddDays(DAYS_PER_WEEK);

            var logs = await this.analyticsRepository.GetWorkoutsInRangeAsync(
                clientId,
                weekStart.ToDateTime(TimeOnly.MinValue),
                weekEnd.ToDateTime(TimeOnly.MinValue));

            buckets.Add(new ConsistencyWeekBucket
            {
                WeekStart = weekStart,
                WorkoutCount = logs.Count,
            });
        }

        return buckets;
    }

    public async Task<DashboardSummary> GetDashboardSummaryAsync(int clientId)
    {
        int totalWorkouts = await this.analyticsRepository.GetTotalWorkoutsAsync(clientId);

        var sevenDaysAgo = DateTime.Today.AddDays(-SEVEN_DAYS);
        var tomorrow = DateTime.Today.AddDays(1);
        var recentLogs = await this.analyticsRepository.GetWorkoutsInRangeAsync(clientId, sevenDaysAgo, tomorrow);

        var totalActiveTime = TimeSpan.Zero;
        var workoutNameCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        foreach (var log in recentLogs)
        {
            totalActiveTime += log.Duration;

            if (!string.IsNullOrWhiteSpace(log.WorkoutName))
            {
                workoutNameCounts.TryGetValue(log.WorkoutName, out int count);
                workoutNameCounts[log.WorkoutName] = count + 1;
            }
        }

        string? preferredWorkout = workoutNameCounts.Count > 0
            ? workoutNameCounts.MaxBy(kvp => kvp.Value).Key
            : null;

        return new DashboardSummary
        {
            TotalWorkouts = totalWorkouts,
            TotalActiveTimeLastSevenDays = totalActiveTime,
            PreferredWorkoutName = preferredWorkout,
        };
    }

    internal static DateOnly GetMondayOfWeek(DateOnly date)
    {
        var dayOfWeek = date.DayOfWeek;
        int offset = dayOfWeek == DayOfWeek.Sunday ? 6 : (int)dayOfWeek - (int)DayOfWeek.Monday;
        return date.AddDays(-offset);
    }
}
