using ClassLibrary.Models;

namespace ClassLibrary.Repositories
{
    public interface IWorkoutAnalyticsRepository
    {
        Task<int> GetTotalWorkoutsAsync(int clientId, CancellationToken cancellationToken = default);
        Task<(IReadOnlyList<WorkoutLog> WorkoutLogs, int TotalCount)> GetWorkoutHistoryPageAsync(int clientId, int skip, int take, CancellationToken cancellationToken = default);
        Task<WorkoutLog?> GetWorkoutLogWithSetsAsync(int clientId, int workoutLogId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<WorkoutLog>> GetWorkoutsInRangeAsync(int clientId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
        Task SaveWorkoutAsync(WorkoutLog log, CancellationToken cancellationToken = default);
    }
}