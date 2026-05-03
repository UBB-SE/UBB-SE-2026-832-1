using ClassLibrary.DTOs;

namespace WebAPI.IServices;

public interface IWorkoutLogService
{
    Task<IReadOnlyList<WorkoutLogDataTransferObject>> GetWorkoutHistoryAsync(int clientId);

    Task SaveWorkoutLogAsync(WorkoutLogDataTransferObject workoutLog);

    Task UpdateWorkoutLogAsync(WorkoutLogDataTransferObject workoutLog);

    Task<double> GetClientWeightAsync(int clientId);
}
