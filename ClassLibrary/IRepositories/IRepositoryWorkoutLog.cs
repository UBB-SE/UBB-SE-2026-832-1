using ClassLibrary.Models;

namespace ClassLibrary.IRepositories;

public interface IRepositoryWorkoutLog
{
    Task<IReadOnlyList<WorkoutLog>> GetWorkoutHistoryAsync(int clientId, CancellationToken cancellationToken = default);

    Task<bool> SaveWorkoutLogAsync(WorkoutLog log, CancellationToken cancellationToken = default);

    Task<bool> UpdateWorkoutLogAsync(WorkoutLog log, CancellationToken cancellationToken = default);

    Task<double> GetClientWeightAsync(int clientId, CancellationToken cancellationToken = default);
}
