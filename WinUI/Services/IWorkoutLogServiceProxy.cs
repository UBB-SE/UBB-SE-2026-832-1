using ClassLibrary.DTOs;

namespace WinUI.Services;

public interface IWorkoutLogServiceProxy
{
    Task<IReadOnlyList<WorkoutLogDataTransferObject>> GetWorkoutHistoryAsync(int clientId);

    Task<double> GetClientWeightAsync(int clientId);
}
