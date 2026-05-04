using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ClassLibrary.Data;
using ClassLibrary.Models;
using ClassLibrary.IRepositories;

namespace ClassLibrary.Repositories;

public sealed class WorkoutAnalyticsRepository : IWorkoutAnalyticsRepository
{
    private readonly AppDbContext databaseContext;

    public WorkoutAnalyticsRepository(AppDbContext databaseContext)
    {
        this.databaseContext = databaseContext;
    }

    public async Task SaveWorkoutAsync(WorkoutLog workoutLog)
    {
        await databaseContext.WorkoutLogs.AddAsync(workoutLog);
        await databaseContext.SaveChangesAsync();
    }

    public async Task<int> GetTotalWorkoutsAsync(int clientId)
    {
        return await databaseContext.WorkoutLogs
            .CountAsync(workoutLog => workoutLog.Client.ClientId == clientId);
    }

    public async Task<IReadOnlyList<WorkoutLog>> GetWorkoutsInRangeAsync(
        int clientId, DateTime startDate, DateTime endDate)
    {
        return await databaseContext.WorkoutLogs
            .AsNoTracking()
            .Where(workoutLog => workoutLog.Client.ClientId == clientId && workoutLog.Date >= startDate && workoutLog.Date <= endDate)
            .ToListAsync();
    }

    public async Task<(IReadOnlyList<WorkoutLog> WorkoutLogs, int TotalCount)> GetWorkoutHistoryPageAsync(
        int clientId, int skip, int take)
    {
        var totalCount = await databaseContext.WorkoutLogs
            .CountAsync(workoutLog => workoutLog.Client.ClientId == clientId);

        var workoutLogs = await databaseContext.WorkoutLogs
            .AsNoTracking()
            .Where(workoutLog => workoutLog.Client.ClientId == clientId)
            .OrderByDescending(workoutLog => workoutLog.Date)
            .ThenByDescending(workoutLog => workoutLog.WorkoutLogId)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        return (workoutLogs, totalCount);
    }

    public async Task<WorkoutLog?> GetWorkoutLogWithSetsAsync(
        int clientId, int workoutLogId)
    {
        return await databaseContext.WorkoutLogs
            .AsNoTracking()
            .Include(workoutLog => workoutLog.Exercises)
            .FirstOrDefaultAsync(workoutLog => workoutLog.WorkoutLogId == workoutLogId && workoutLog.Client.ClientId == clientId);
    }
}
