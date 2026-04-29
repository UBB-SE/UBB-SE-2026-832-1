namespace ClassLibrary.Repositories;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ClassLibrary.Data;
using ClassLibrary.Models;
using ClassLibrary.IRepositories;

public sealed class WorkoutAnalyticsRepository : IWorkoutAnalyticsRepository
{
    private readonly AppDbContext _databaseContext;

    public WorkoutAnalyticsRepository(AppDbContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public async Task SaveWorkoutAsync(WorkoutLog workoutLog, CancellationToken cancellationToken = default)
    {
        await _databaseContext.WorkoutLogs.AddAsync(workoutLog, cancellationToken);
        await _databaseContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> GetTotalWorkoutsAsync(int clientId, CancellationToken cancellationToken = default)
    {
        return await _databaseContext.WorkoutLogs
            .CountAsync(workoutLog => workoutLog.Client.ClientId == clientId, cancellationToken);
    }

    public async Task<IReadOnlyList<WorkoutLog>> GetWorkoutsInRangeAsync(
        int clientId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _databaseContext.WorkoutLogs
            .AsNoTracking()
            .Where(workoutLog => workoutLog.Client.ClientId == clientId && workoutLog.Date >= startDate && workoutLog.Date <= endDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<WorkoutLog> WorkoutLogs, int TotalCount)> GetWorkoutHistoryPageAsync(
        int clientId, int skip, int take, CancellationToken cancellationToken = default)
    {
        var totalCount = await _databaseContext.WorkoutLogs
            .CountAsync(workoutLog => workoutLog.Client.ClientId == clientId, cancellationToken);

        var workoutLogs = await _databaseContext.WorkoutLogs
            .AsNoTracking()
            .Where(workoutLog => workoutLog.Client.ClientId == clientId)
            .OrderByDescending(workoutLog => workoutLog.Date)
            .ThenByDescending(workoutLog => workoutLog.WorkoutLogId)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        return (workoutLogs, totalCount);
    }

    public async Task<WorkoutLog?> GetWorkoutLogWithSetsAsync(
        int clientId, int workoutLogId, CancellationToken cancellationToken = default)
    {
        return await _databaseContext.WorkoutLogs
            .AsNoTracking()
            .Include(workoutLog => workoutLog.Exercises)
            .FirstOrDefaultAsync(workoutLog => workoutLog.WorkoutLogId == workoutLogId && workoutLog.Client.ClientId == clientId, cancellationToken);
    }
}