using System.Net.Http.Json;
using ClassLibrary.DTOs;
using ClassLibrary.Models;

namespace WinUI.Services;

public sealed class WorkoutLogServiceProxy : IWorkoutLogServiceProxy
{
    private readonly HttpClient httpClient;

    public WorkoutLogServiceProxy(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<IReadOnlyList<WorkoutLogDataTransferObject>> GetWorkoutHistoryAsync(int clientId)
    {
        var history = await this.httpClient.GetFromJsonAsync<List<WorkoutLogDataTransferObject>>(
            $"{ApiBaseUrl.BASE_URL}/api/client/{clientId}/workout-history");
        return history ?? [];
    }

    public async Task<double> GetClientWeightAsync(int clientId)
    {
        var userData = await this.httpClient.GetFromJsonAsync<UserDataDto>(
            $"{ApiBaseUrl.BASE_URL}/api/users/{clientId}/data");
        return userData?.Weight ?? 0;
    }

    public async Task<bool> UpdateWorkoutLogAsync(WorkoutLog workoutLog)
    {
        var dto = MapToWorkoutLogDataTransferObject(workoutLog);
        var response = await this.httpClient.PutAsJsonAsync(
            $"{ApiBaseUrl.BASE_URL}/api/client/modify-workout", dto);
        return response.IsSuccessStatusCode;
    }

    private static WorkoutLogDataTransferObject MapToWorkoutLogDataTransferObject(WorkoutLog log)
    {
        return new WorkoutLogDataTransferObject
        {
            WorkoutLogId = log.WorkoutLogId,
            Client = new ClientDataTransferObject
            {
                ClientId = log.Client?.ClientId ?? 0,
                Email = log.Client?.Email ?? string.Empty,
                FullName = log.Client?.FullName ?? string.Empty,
            },
            WorkoutName = log.WorkoutName,
            Date = log.Date,
            Duration = log.Duration,
            SourceTemplateId = log.SourceTemplateId,
            Type = log.Type.ToString(),
            Exercises = log.Exercises.Select(e => new LoggedExerciseDataTransferObject
            {
                LoggedExerciseId = e.LoggedExerciseId,
                ExerciseName = e.ExerciseName,
                TargetMuscles = e.TargetMuscles.ToString(),
                IsSystemAdjusted = e.IsSystemAdjusted,
                AdjustmentNote = e.AdjustmentNote,
                PerformanceRatio = e.PerformanceRatio,
                Sets = e.Sets.Select(s => new LoggedSetDataTransferObject
                {
                    LoggedSetId = s.LoggedSetId,
                    ExerciseName = s.ExerciseName,
                    SetIndex = s.SetIndex,
                    SetNumber = s.SetNumber,
                    ActualReps = s.ActualReps ?? 0,
                    ActualWeight = (float)(s.ActualWeight ?? 0),
                    TargetReps = s.TargetReps ?? 0,
                    TargetWeight = (float)(s.TargetWeight ?? 0),
                }).ToList(),
            }).ToList(),
            TotalCaloriesBurned = log.TotalCaloriesBurned,
            AverageMetabolicEquivalent = log.AverageMetabolicEquivalent,
            IntensityTag = log.IntensityTag,
            Rating = log.Rating,
            TrainerNotes = log.TrainerNotes,
        };
    }
}
