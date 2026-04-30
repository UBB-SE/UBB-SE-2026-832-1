using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClassLibrary.DTOs;

namespace WebAPI.Services;

public interface ITrainerService
{
    Task<IReadOnlyList<ClientDto>> GetAssignedClientsAsync(int trainerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WorkoutHistoryResponseDto>> GetClientWorkoutHistoryAsync(int clientId, CancellationToken cancellationToken = default);
    Task<bool> SaveWorkoutFeedbackAsync(WorkoutFeedbackRequestDto request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WorkoutTemplateDto>> GetAvailableWorkoutsAsync(int clientId, CancellationToken cancellationToken = default);
    Task<bool> DeleteWorkoutTemplateAsync(int templateId, CancellationToken cancellationToken = default);
    Task<(bool Success, string ErrorMessage)> AssignNewRoutineAsync(RoutineRequestDto request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetAllExerciseNamesAsync(CancellationToken cancellationToken = default);
    Task<bool> CreateAndAssignNutritionPlanAsync(NutritionPlanRequestDto request, CancellationToken cancellationToken = default);
}
