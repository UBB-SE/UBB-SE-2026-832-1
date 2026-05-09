using System.Net.Http.Json;
using ClassLibrary.DTOs;
using ClassLibrary.Models;

namespace WinUI.Services;

public sealed class CreateWorkoutService : ICreateWorkoutService
{
    private readonly HttpClient httpClient;
    private const string ApiUrl = ApiBaseUrl.BASE_URL + "/api";
    private const string Route = "trainer";

    public CreateWorkoutService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<IReadOnlyList<string>> GetAllExerciseNamesAsync()
    {
        var response = await this.httpClient.GetFromJsonAsync<List<string>>($"{ApiUrl}/{Route}/exercise-names");
        return response ?? new List<string>();
    }

    public async Task SaveTrainerWorkoutAsync(WorkoutTemplate workoutTemplate)
    {
        var dto = new WorkoutTemplateDataTransferObject
        {
            WorkoutTemplateId = workoutTemplate.WorkoutTemplateId,
            Name = workoutTemplate.Name,
            Type = workoutTemplate.Type.ToString(),
            ClientId = workoutTemplate.Client.ClientId,
            Exercises = workoutTemplate.Exercises.Select(exercise => new TemplateExerciseDataTransferObject
            {
                Name = exercise.Name,
                MuscleGroup = exercise.MuscleGroup.ToString(),
                TargetSets = exercise.TargetSets,
                TargetReps = exercise.TargetReps,
                TargetWeight = exercise.TargetWeight,
            }).ToList(),
        };

        var response = await this.httpClient.PostAsJsonAsync($"{ApiUrl}/{Route}/save-workout", dto);
        response.EnsureSuccessStatusCode();
    }
}
