using ClassLibrary.Models;

namespace ClassLibrary.IRepositories;

public interface IWorkoutTemplateRepository
{
    Task<IReadOnlyList<WorkoutTemplate>> GetAvailableWorkoutsAsync(int clientId, CancellationToken cancellationToken = default);

    Task<WorkoutTemplate?> GetByIdAsync(int workoutTemplateId, CancellationToken cancellationToken = default);
}
