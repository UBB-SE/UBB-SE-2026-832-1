using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClassLibrary.Models;

namespace ClassLibrary.IRepositories;

public interface IWorkoutAnalyticsRepository
{
    Task SaveWorkoutAsync(WorkoutLog workoutLog, CancellationToken cancellationToken = default);
    Task<int> GetTotalWorkoutsAsync(int clientId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WorkoutLog>> GetWorkoutsInRangeAsync(int clientId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<WorkoutLog> WorkoutLogs, int TotalCount)> GetWorkoutHistoryPageAsync(int clientId, int skip, int take, CancellationToken cancellationToken = default);
    Task<WorkoutLog?> GetWorkoutLogWithSetsAsync(int clientId, int workoutLogId, CancellationToken cancellationToken = default);
}