using ClassLibrary.Models;

namespace ClassLibrary.IRepositories;

public interface IWorkoutAnalyticsStore
{
    Task<int> SaveWorkoutAsync(long userId, WorkoutLog log, CancellationToken cancellationToken);
}