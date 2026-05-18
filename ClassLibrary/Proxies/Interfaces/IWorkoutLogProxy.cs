using ClassLibrary.DTOs;

namespace ClassLibrary.Proxies.Interfaces;

public interface IWorkoutLogProxy
{
    Task<IReadOnlyList<WorkoutLogDataTransferObject>> GetWorkoutHistoryAsync(int clientId);

    Task<double> GetClientWeightAsync(int clientId);
}



