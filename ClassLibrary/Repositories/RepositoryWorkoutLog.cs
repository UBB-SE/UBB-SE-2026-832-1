using ClassLibrary.IRepositories;
using ClassLibrary.Models;

namespace ClassLibrary.Repositories;

public sealed class RepositoryWorkoutLog : IRepositoryWorkoutLog
{
    public Task<IReadOnlyList<WorkoutLog>> GetWorkoutHistoryAsync(int clientId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> SaveWorkoutLogAsync(WorkoutLog log, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateWorkoutLogAsync(WorkoutLog log, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<double> GetClientWeightAsync(int clientId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
