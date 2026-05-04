using System.Net.Http.Json;
using ClassLibrary.DTOs;

namespace WinUI.Services;

public sealed class ActiveWorkoutService : IActiveWorkoutService
{
    private readonly HttpClient httpClient;
    private const string BaseRoute = "https://localhost:7197/api/client";

    public ActiveWorkoutService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<IReadOnlyList<WorkoutTemplateDataTransferObject>> GetAvailableWorkoutsForClient(int clientId)
    {
        var availableWorkoutsResponse = await this.httpClient.GetFromJsonAsync<List<WorkoutTemplateDataTransferObject>>($"{BaseRoute}/{clientId}/available-workouts");
        return availableWorkoutsResponse ?? new List<WorkoutTemplateDataTransferObject>();
    }

    public async Task<IReadOnlyList<WorkoutTemplateDataTransferObject>> GetCustomAndTrainerAssignedWorkoutsForClient(int clientId)
    {
        var customAndTrainerWorkoutsResponse = await this.httpClient.GetFromJsonAsync<List<WorkoutTemplateDataTransferObject>>($"{BaseRoute}/{clientId}/available-workouts");
        return customAndTrainerWorkoutsResponse ?? new List<WorkoutTemplateDataTransferObject>();
    }

    public async Task<WorkoutTemplateDataTransferObject?> FindWorkoutTemplateById(int clientId, int? id)
    {
        if (!id.HasValue)
        {
            return null;
        }

        var availableWorkoutTemplates = await GetAvailableWorkoutsForClient(clientId);
        return availableWorkoutTemplates.FirstOrDefault(workoutTemplate => workoutTemplate.WorkoutTemplateId == id.Value);
    }

    public async Task<IDictionary<string, double>> GetPreviousBestWeights(int clientId)
    {
        var response = await this.httpClient.GetFromJsonAsync<PreviousBestWeightsDataTransferObject>($"{BaseRoute}/{clientId}/previous-best-weights");
        return response?.BestWeightsByExercise ?? new Dictionary<string, double>();
    }

    public async Task<bool> SaveSetAsync(WorkoutLogDataTransferObject workoutLog, string exerciseName, LoggedExerciseDataTransferObject set)
    {
        var dto = new FinalizeWorkoutRequestDataTransferObject { WorkoutLog = workoutLog };
        var response = await this.httpClient.PostAsJsonAsync($"{BaseRoute}/finalize-workout", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> FinalizeWorkoutAsync(WorkoutLogDataTransferObject workoutLog)
    {
        var dto = new FinalizeWorkoutRequestDataTransferObject { WorkoutLog = workoutLog };
        var response = await this.httpClient.PostAsJsonAsync($"{BaseRoute}/finalize-workout", dto);
        return response.IsSuccessStatusCode;
    }
}
