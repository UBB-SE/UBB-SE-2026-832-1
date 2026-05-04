namespace WebAPI.Services;

using WebAPI.Services.Interfaces;
using ClassLibrary.Models;
using ClassLibrary.DTOs;
using ClassLibrary.IRepositories;

public sealed class WorkoutDataForwarder : IWorkoutDataForwarder
{
    private readonly IWorkoutAnalyticsStore workoutAnalyticsStore;
    private readonly IAnalyticsDashboardRefreshBus analyticsDashboardRefreshBus;

    private const float LightThreshold = 3.0f;
    private const float ModerateThreshold = 6.0f;

    private const string LightIntensity = "light";
    private const string ModerateIntensity = "moderate";
    private const string IntenseIntensity = "intense";

    public WorkoutDataForwarder(
        IWorkoutAnalyticsStore workoutAnalyticsStore,
        IAnalyticsDashboardRefreshBus analyticsDashboardRefreshBus)
    {
        this.workoutAnalyticsStore = workoutAnalyticsStore;
        this.analyticsDashboardRefreshBus = analyticsDashboardRefreshBus;
    }

    public async Task<WorkoutForwardResponseDto> ForwardCompletedWorkoutAsync(
        WorkoutLogRequestDto request,
        CancellationToken cancellationToken = default)
    {
        // Mapping DTO to Domain Model for internal logic
        var workoutLog = new WorkoutLog
        {
            ClientId = request.UserId,
            Exercises = request.Exercises.Select(exercise => new Exercise
            {
                ExerciseCaloriesBurned = exercise.CaloriesBurned,
                MetabolicEquivalent = exercise.MetabolicEquivalent
            }).ToList()
        };

        workoutLog.TotalCaloriesBurned = workoutLog.Exercises.Sum(exercise => exercise.ExerciseCaloriesBurned);

        if (workoutLog.Exercises.Count > 0)
        {
            workoutLog.AverageMetabolicEquivalent = workoutLog.Exercises.Average(exercise => exercise.MetabolicEquivalent);
            workoutLog.IntensityTag = CalculateIntensityTag(workoutLog.AverageMetabolicEquivalent);
        }

        int logId = await workoutAnalyticsStore.SaveWorkoutAsync(workoutLog.ClientId, workoutLog, cancellationToken);

        analyticsDashboardRefreshBus.RequestRefresh();

        return new WorkoutForwardResponseDto { LogId = logId };
    }

    private static string CalculateIntensityTag(float averageMetabolicEquivalent)
    {
        if (averageMetabolicEquivalent < LightThreshold)
        {
            return LightIntensity;
        }

        if (averageMetabolicEquivalent < ModerateThreshold)
        {
            return ModerateIntensity;
        }

        return IntenseIntensity;
    }
}