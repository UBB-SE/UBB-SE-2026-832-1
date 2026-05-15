using System.Net.Http.Json;
using ClassLibrary.DTOs;
using ClassLibrary.Models;
using ClassLibrary.Proxies.Interfaces;

namespace ClassLibrary.Proxies;

public sealed class TrainerDashboardProxy : ITrainerDashboardProxy
{
    private readonly HttpClient httpClient;
    private const string BaseAddress = ApiBaseUrl.BASE_URL + "/api";
    private const string TrainerRoute = "trainer";

    public TrainerDashboardProxy(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<IReadOnlyList<Client>> GetAssignedClientsAsync(int trainerId)
    {
        var clientDataTransferObjects = await this.httpClient.GetFromJsonAsync<List<ClientDataTransferObject>>($"{BaseAddress}/{TrainerRoute}/{trainerId}/assigned-clients");
        return DataTransferObjectToDomainModelMappers.MapClients(clientDataTransferObjects);
    }

    public async Task<IReadOnlyList<WorkoutLog>> GetClientWorkoutHistoryAsync(int clientId)
    {
        var workoutLogDataTransferObjects = await this.httpClient.GetFromJsonAsync<List<WorkoutLogDataTransferObject>>($"{BaseAddress}/{TrainerRoute}/{clientId}/workout-history");
        return DataTransferObjectToDomainModelMappers.MapWorkoutLogs(workoutLogDataTransferObjects);
    }

    public async Task<IReadOnlyList<WorkoutTemplate>> GetAvailableWorkoutsAsync(int clientId)
    {
        var workoutTemplateDataTransferObjects = await this.httpClient.GetFromJsonAsync<List<WorkoutTemplateDataTransferObject>>($"{BaseAddress}/{TrainerRoute}/{clientId}/available-workouts");
        return DataTransferObjectToDomainModelMappers.MapWorkoutTemplates(workoutTemplateDataTransferObjects);
    }

    public async Task<IReadOnlyList<string>> GetAllExerciseNamesAsync()
    {
        var exerciseNames = await this.httpClient.GetFromJsonAsync<List<string>>($"{BaseAddress}/{TrainerRoute}/exercise-names");
        return exerciseNames ?? new List<string>();
    }

    public async Task<bool> DeleteWorkoutTemplateAsync(int templateId)
    {
        var deleteWorkoutTemplateResponse = await this.httpClient.DeleteAsync($"{BaseAddress}/trainer/workout-template/{templateId}");
        return deleteWorkoutTemplateResponse.IsSuccessStatusCode;
    }

    public async Task<bool> AssignNewRoutineAsync(int templateId, int clientId, string name, IReadOnlyList<TemplateExercise> exercises)
    {
        var assignNewRoutineRequestDataTransferObject = new AssignNewRoutineRequestDataTransferObject
        {
            EditingTemplateId = templateId,
            ClientId = clientId,
            RoutineName = name,
            Exercises = exercises.Select(exercise => new TemplateExerciseDataTransferObject
            {
                Name = exercise.Name,
                MuscleGroup = exercise.MuscleGroup.ToString(),
                TargetSets = exercise.TargetSets,
                TargetReps = exercise.TargetReps,
                TargetWeight = exercise.TargetWeight,
            }).ToList()
        };

        var assignNewRoutineResponse = await this.httpClient.PostAsJsonAsync($"{BaseAddress}/trainer/assign-new-routine", assignNewRoutineRequestDataTransferObject);
        return assignNewRoutineResponse.IsSuccessStatusCode;
    }

    public async Task SaveWorkoutFeedbackAsync(WorkoutLog workoutLog)
    {
        var saveWorkoutFeedbackRequestDataTransferObject = new SaveWorkoutFeedbackRequestDataTransferObject
        {
            WorkoutLogId = workoutLog.WorkoutLogId,
            Rating = workoutLog.Rating,
            TrainerNotes = workoutLog.TrainerNotes,
        };

        await this.httpClient.PutAsJsonAsync($"{BaseAddress}/trainer/save-workout-feedback", saveWorkoutFeedbackRequestDataTransferObject);
    }

    public async Task<bool> CreateAndAssignNutritionPlanAsync(DateTime startDate, DateTime endDate, int clientId)
    {
        var createNutritionPlanRequestDataTransferObject = new CreateNutritionPlanRequestDataTransferObject
        {
            StartDate = startDate,
            EndDate = endDate,
            ClientId = clientId,
        };

        var createAndAssignNutritionPlanResponse = await this.httpClient.PostAsJsonAsync($"{BaseAddress}/trainer/create-assign-nutrition-plan", createNutritionPlanRequestDataTransferObject);
        return createAndAssignNutritionPlanResponse.IsSuccessStatusCode;
    }
}


