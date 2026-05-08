using ClassLibrary.DTOs;
using ClassLibrary.Models;

namespace WinUI.Services;

public interface ITrainerDashboardService
{
    Task<IReadOnlyList<Client>> GetAssignedClientsAsync(int trainerId);

    Task<IReadOnlyList<WorkoutLog>> GetClientWorkoutHistoryAsync(int clientId);

    Task<IReadOnlyList<WorkoutTemplate>> GetAvailableWorkoutsAsync(int clientId);

    Task<IReadOnlyList<string>> GetAllExerciseNamesAsync();

    Task<bool> DeleteWorkoutTemplateAsync(int templateId);

    Task<bool> AssignNewRoutineAsync(int templateId, int clientId, string name, IReadOnlyList<TemplateExercise> exercises);

    Task SaveWorkoutFeedbackAsync(WorkoutLog workoutLog);

    Task<bool> CreateAndAssignNutritionPlanAsync(DateTime startDate, DateTime endDate, int clientId);
}
