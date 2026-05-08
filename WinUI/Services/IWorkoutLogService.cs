using ClassLibrary.DTOs;
using ClassLibrary.Models;

namespace WinUI.Services;

public interface IWorkoutLogService
{
    Task<IReadOnlyList<WorkoutLogDataTransferObject>> GetWorkoutHistoryAsync(int clientId);

    Task<double> GetClientWeightAsync(int clientId);

    Task<bool> UpdateWorkoutLogAsync(WorkoutLog workoutLog);
}
