using ClassLibrary.Models;

namespace ClassLibrary.IRepositories;

public interface IWorkoutLogRepository
{
    Task<IReadOnlyList<WorkoutLog>> GetWorkoutHistoryAsync(int clientId, CancellationToken cancellationToken = default);

    Task SaveWorkoutLogAsync(WorkoutLog log, CancellationToken cancellationToken = default);

    Task UpdateWorkoutLogAsync(WorkoutLog log, CancellationToken cancellationToken = default);

    Task<double> GetClientWeightAsync(int clientId, CancellationToken cancellationToken = default);
}
