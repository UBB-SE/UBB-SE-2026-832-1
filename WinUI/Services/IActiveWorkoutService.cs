using ClassLibrary.DTOs;

namespace WinUI.Services;

public interface IActiveWorkoutService
{
    Task<IReadOnlyList<WorkoutTemplateDataTransferObject>> GetAvailableWorkoutsForClient(int clientId);
    Task<IReadOnlyList<WorkoutTemplateDataTransferObject>> GetCustomAndTrainerAssignedWorkoutsForClient(int clientId);
    Task<WorkoutTemplateDataTransferObject?> FindWorkoutTemplateById(int clientId, int? id);
    Task<IDictionary<string, double>> GetPreviousBestWeightsAsync(int clientId);
    Task<bool> SaveSetAsync(WorkoutLogDataTransferObject workoutLog, string exerciseName, LoggedExerciseDataTransferObject set);
    Task<bool> FinalizeWorkoutAsync(WorkoutLogDataTransferObject workoutLog);
}
