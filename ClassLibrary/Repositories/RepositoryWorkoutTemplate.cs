using ClassLibrary.IRepositories;
using ClassLibrary.Models;

namespace ClassLibrary.Repositories;

public sealed class RepositoryWorkoutTemplate : IRepositoryWorkoutTemplate
{
    public Task<IReadOnlyList<WorkoutTemplate>> GetAvailableWorkoutsAsync(int clientId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<WorkoutTemplate?> GetByIdAsync(int workoutTemplateId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
