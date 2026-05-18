using System.Net.Http.Json;
using ClassLibrary.DTOs;
using ClassLibrary.DTOs.Analytics;
using ClassLibrary.Proxies.Interfaces;

namespace ClassLibrary.Proxies;

public sealed class ClientDashboardProxy : IClientDashboardProxy
{
    private const string ROUTE = "api/client";
    private readonly HttpClient httpClient;

    public ClientDashboardProxy(HttpClient httpClient)
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

            var exerciseCalories = exercises
                .Where(exercise => !string.IsNullOrWhiteSpace(exercise.ExerciseName))
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

            var caloriesByExercise = exerciseCalories
                .ToDictionary(item => item.ExerciseName, item => item.CaloriesBurned, StringComparer.OrdinalIgnoreCase);

            foreach (var exerciseName in sets
                .Select(set => set.ExerciseName)
                .Where(exerciseName => !string.IsNullOrWhiteSpace(exerciseName))
                .Distinct(StringComparer.OrdinalIgnoreCase))
            {
                if (!caloriesByExercise.ContainsKey(exerciseName))
                {
                    caloriesByExercise[exerciseName] = 0;
                }
            }

            exerciseCalories = caloriesByExercise
                .Select(pair => new ExerciseCalorieInfo
                {
                    ExerciseName = pair.Key,
                    CaloriesBurned = pair.Value,
                })
                .OrderBy(item => item.ExerciseName, StringComparer.OrdinalIgnoreCase)
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
            return new List<AchievementDataTransferObject>();
        }
    }
}


