using ClassLibrary.DTOs;

namespace WebAPI.IServices;

public interface ITrainerService
{
    Task<IReadOnlyList<ClientDataTransferObject>> GetAssignedClientsAsync(int trainerId);

    Task<IReadOnlyList<WorkoutLogDataTransferObject>> GetClientWorkoutHistoryAsync(int clientId);

    Task<bool> SaveWorkoutFeedbackAsync(SaveWorkoutFeedbackRequestDataTransferObject request);

    Task<IReadOnlyList<WorkoutTemplateDataTransferObject>> GetAvailableWorkoutsAsync(int clientId);

    Task<bool> DeleteWorkoutTemplateAsync(int templateId);

    Task<bool> SaveTrainerWorkoutAsync(WorkoutTemplateDataTransferObject templateDto);

    Task<(bool Success, string ErrorMessage)> AssignNewRoutineAsync(AssignNewRoutineRequestDataTransferObject request);

    Task<IReadOnlyList<string>> GetAllExerciseNamesAsync();

    Task<bool> AssignNutritionPlanAsync(AssignNutritionPlanRequestDataTransferObject request);

    Task<bool> CreateAndAssignNutritionPlanAsync(CreateNutritionPlanRequestDataTransferObject request);
}
