using ClassLibrary.DTOs;
using ClassLibrary.Models;

namespace WinUI.Services;

public sealed class WorkoutLogService : IWorkoutLogService
{
    private readonly IWorkoutLogServiceProxy serviceProxy;

    public WorkoutLogService(IWorkoutLogServiceProxy serviceProxy)
    {
        this.serviceProxy = serviceProxy;
    }

    public Task<IReadOnlyList<WorkoutLogDataTransferObject>> GetWorkoutHistoryAsync(int clientId)
    {
        return this.serviceProxy.GetWorkoutHistoryAsync(clientId);
    }

    public Task<double> GetClientWeightAsync(int clientId)
    {
        return this.serviceProxy.GetClientWeightAsync(clientId);
    }

    public Task<bool> UpdateWorkoutLogAsync(WorkoutLog workoutLog)
    {
        return this.serviceProxy.UpdateWorkoutLogAsync(workoutLog);
    }
}
