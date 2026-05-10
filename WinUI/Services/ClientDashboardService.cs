using System.Net.Http.Json;
using ClassLibrary.DTOs;
using ClassLibrary.DTOs.Analytics;

namespace WinUI.Services;

public sealed class ClientDashboardService : IClientDashboardService
{
    private const string ROUTE = "api/client";
    private readonly HttpClient httpClient;

    public ClientDashboardService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<DashboardSummary> GetDashboardSummaryAsync(int clientId)
    {
        try
        {
            var result = await httpClient.GetFromJsonAsync<DashboardSummary>(
                $"{ApiBaseUrl.BASE_URL}/{ROUTE}/{clientId}/dashboard-summary");
            return result ?? new DashboardSummary();
        }
        catch
        {
            // Return safe empty data on error
            return new DashboardSummary();
        }
    }

    public async Task<IReadOnlyList<ConsistencyWeekBucket>> GetConsistencyLastFourWeeksAsync(int clientId)
    {
        try
        {
            var result = await httpClient.GetFromJsonAsync<List<ConsistencyWeekBucket>>(
                $"{ApiBaseUrl.BASE_URL}/{ROUTE}/{clientId}/consistency-four-weeks");
            return result ?? new List<ConsistencyWeekBucket>();
        }
        catch
        {
            // Return safe empty data on error
            return new List<ConsistencyWeekBucket>();
        }
    }

    public async Task<WorkoutHistoryPageResult> GetWorkoutHistoryPageAsync(int clientId, int page, int pageSize)
    {
        try
        {
            var result = await httpClient.GetFromJsonAsync<WorkoutHistoryPageResult>(
                $"{ApiBaseUrl.BASE_URL}/{ROUTE}/{clientId}/workout-history-page?page={page}&pageSize={pageSize}");
            return result ?? new WorkoutHistoryPageResult { TotalCount = 0, Items = new List<WorkoutHistoryRow>() };
        }
        catch
        {
            // Return safe empty data on error
            return new WorkoutHistoryPageResult { TotalCount = 0, Items = new List<WorkoutHistoryRow>() };
        }
    }

    public async Task<WorkoutSessionDetail?> GetWorkoutSessionDetailAsync(int clientId, int workoutLogId)
    {
        try
        {
            var history = await httpClient.GetFromJsonAsync<List<WorkoutLogDataTransferObject>>(
                $"{ApiBaseUrl.BASE_URL}/{ROUTE}/{clientId}/workout-history");

            var log = history?.FirstOrDefault(item => item.WorkoutLogId == workoutLogId);
            if (log == null)
            {
                return null;
            }

            var exercises = log.Exercises ?? new List<LoggedExerciseDataTransferObject>();

            // Keep 0-calorie exercises visible in the breakdown.
            var exerciseCalories = exercises
                .GroupBy(exercise => exercise.ExerciseName)
                .Select(group => new ExerciseCalorieInfo
                {
                    ExerciseName = group.Key,
                    CaloriesBurned = Math.Max(0, group.Sum(exercise => exercise.ExerciseCaloriesBurned)),
                })
                .ToList();

            var sets = exercises
                .SelectMany(exercise => exercise.Sets)
                .Select(set => new WorkoutSetRow
                {
                    ExerciseName = set.ExerciseName,
                    SetIndex = set.SetIndex,
                    ActualReps = set.ActualReps,
                    ActualWeight = set.ActualWeight,
                })
                .ToList();

            return new WorkoutSessionDetail
            {
                WorkoutLogId = log.WorkoutLogId,
                WorkoutName = log.WorkoutName,
                LogDate = log.Date,
                DurationSeconds = (int)Math.Max(0, log.Duration.TotalSeconds),
                TotalCaloriesBurned = log.TotalCaloriesBurned,
                IntensityTag = log.IntensityTag,
                ExerciseCalories = exerciseCalories,
                Sets = sets,
            };
        }
        catch
        {
            return null;
        }
    }

    public async Task<IReadOnlyList<AchievementDataTransferObject>> GetRecentAchievementsAsync(int clientId)
    {
        try
        {
            var result = await httpClient.GetFromJsonAsync<List<AchievementDataTransferObject>>(
                $"{ApiBaseUrl.BASE_URL}/{ROUTE}/{clientId}/achievements");
            return result ?? new List<AchievementDataTransferObject>();
        }
        catch
        {
            // Return safe empty data on error
            return new List<AchievementDataTransferObject>();
        }
    }
}
