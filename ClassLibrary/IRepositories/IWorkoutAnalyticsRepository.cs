using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Models;

namespace ClassLibrary.IRepositories;

public interface IWorkoutAnalyticsRepository
{
    Task SaveWorkoutAsync(WorkoutLog workoutLog);
    Task<int> GetTotalWorkoutsAsync(int clientId);
    Task<IReadOnlyList<WorkoutLog>> GetWorkoutsInRangeAsync(int clientId, DateTime startDate, DateTime endDate);
    Task<(IReadOnlyList<WorkoutLog> WorkoutLogs, int TotalCount)> GetWorkoutHistoryPageAsync(int clientId, int skip, int take);
    Task<WorkoutLog?> GetWorkoutLogWithSetsAsync(int clientId, int workoutLogId);
}
