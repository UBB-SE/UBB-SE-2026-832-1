using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.DTOs;

namespace WebAPI.Services;

public interface ITrainerService
{
    Task<List<ClientDto>> GetAssignedClientsAsync(int trainerId);
    Task<List<WorkoutHistoryResponseDto>> GetClientWorkoutHistoryAsync(int clientId);
    Task<bool> SaveWorkoutFeedbackAsync(WorkoutFeedbackRequestDto request);
    Task<List<WorkoutTemplateDto>> GetAvailableWorkoutsAsync(int clientId);
    Task<bool> DeleteWorkoutTemplateAsync(int templateId);
    Task<(bool Success, string ErrorMessage)> AssignNewRoutineAsync(RoutineRequestDto request);
    Task<List<string>> GetAllExerciseNamesAsync();
    Task<bool> CreateAndAssignNutritionPlanAsync(NutritionPlanRequestDto request);
}
