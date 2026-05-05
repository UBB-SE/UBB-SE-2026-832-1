using System.Net.Http.Json;
using ClassLibrary.DTOs;
using ClassLibrary.Models;

namespace WinUI.Services;

public sealed class TrainerDashboardService : ITrainerDashboardService
{
    private readonly HttpClient httpClient;
    private const string ApiUrl = "https://localhost:7197/api";
    private const string Route = "trainer";

    public TrainerDashboardService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<IReadOnlyList<Client>> GetAssignedClientsAsync(int trainerId)
    {
        var response = await this.httpClient.GetFromJsonAsync<List<ClientDataTransferObject>>($"{ApiUrl}/{Route}/{trainerId}/assigned-clients");
        var dtos = response ?? new List<ClientDataTransferObject>();
        return dtos.Select(dto => new Client
        {
            Id = dto.ClientId,
            Username = dto.Username,
        }).ToList();
    }

    public async Task<IReadOnlyList<WorkoutLog>> GetClientWorkoutHistoryAsync(int clientId)
    {
        var response = await this.httpClient.GetFromJsonAsync<List<WorkoutLogDataTransferObject>>($"{ApiUrl}/client/{clientId}/workout-history");
        var dtos = response ?? new List<WorkoutLogDataTransferObject>();
        return dtos.Select(dto => new WorkoutLog
        {
            WorkoutLogId = dto.WorkoutLogId,
            Date = dto.Date,
            WorkoutName = dto.WorkoutName,
            Duration = dto.Duration,
            TotalCaloriesBurned = dto.TotalCaloriesBurned,
            IntensityTag = dto.IntensityTag,
        }).ToList();
    }

    public async Task<IReadOnlyList<WorkoutTemplate>> GetAvailableWorkoutsAsync(int clientId)
    {
        var response = await this.httpClient.GetFromJsonAsync<List<WorkoutTemplateDataTransferObject>>($"{ApiUrl}/client/{clientId}/available-workouts");
        var dtos = response ?? new List<WorkoutTemplateDataTransferObject>();
        return dtos.Select(dto => new WorkoutTemplate
        {
            WorkoutTemplateId = dto.WorkoutTemplateId,
            Name = dto.Name,
            Type = Enum.TryParse<WorkoutType>(dto.Type, true, out var parsed) ? parsed : WorkoutType.Custom,
        }).ToList();
    }

    public async Task<IReadOnlyList<string>> GetAllExerciseNamesAsync()
    {
        var response = await this.httpClient.GetFromJsonAsync<List<string>>($"{ApiUrl}/trainer/exercises");
        return response ?? new List<string>();
    }

    public async Task<bool> DeleteWorkoutTemplateAsync(int templateId)
    {
        var response = await this.httpClient.DeleteAsync($"{ApiUrl}/trainer/workout-template/{templateId}");
        return response.IsSuccessStatusCode;
    }

    public async Task<OperationResult> AssignNewRoutineAsync(int templateId, int clientId, string name, IReadOnlyList<TemplateExercise> exercises)
    {
        var dto = new AssignRoutineRequestDataTransferObject
        {
            TemplateId = templateId,
            ClientId = clientId,
            Name = name,
            Exercises = exercises.Select(e => new TemplateExerciseDataTransferObject
            {
                Name = e.Name,
                MuscleGroup = e.MuscleGroup.ToString(),
                TargetSets = e.TargetSets,
                TargetReps = e.TargetReps,
                TargetWeight = e.TargetWeight,
            }).ToList()
        };

        var response = await this.httpClient.PostAsJsonAsync($"{ApiUrl}/trainer/assign-routine", dto);
        if (!response.IsSuccessStatusCode)
        {
            return new OperationResult { Success = false, ErrorMessage = "Failed to assign" };
        }

        return new OperationResult { Success = true };
    }

    public async Task SaveWorkoutFeedbackAsync(WorkoutLog workoutLog)
    {
        await this.httpClient.PostAsJsonAsync($"{ApiUrl}/trainer/workout-feedback", workoutLog);
    }

    public async Task<bool> CreateAndAssignNutritionPlanAsync(DateTime startDate, DateTime endDate, int clientId)
    {
        var dto = new NutritionPlanDataTransferObject { StartDate = startDate, EndDate = endDate };
        var response = await this.httpClient.PostAsJsonAsync($"{ApiUrl}/trainer/create-nutrition-plan/{clientId}", dto);
        return response.IsSuccessStatusCode;
    }
}
