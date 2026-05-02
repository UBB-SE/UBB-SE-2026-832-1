using ClassLibrary.Models;

namespace ClassLibrary.IRepositories;

public interface IWorkoutLogRepository
{
    Task<IReadOnlyList<WorkoutLog>> GetWorkoutHistoryAsync(int clientId);

    Task SaveWorkoutLogAsync(WorkoutLog log);

    Task UpdateWorkoutLogAsync(WorkoutLog log);

    Task<double> GetClientWeightAsync(int clientId);
}
