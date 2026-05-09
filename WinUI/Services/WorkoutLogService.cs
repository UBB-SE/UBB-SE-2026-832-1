using ClassLibrary.DTOs;
using WinUI.Services.Interfaces;

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
}
