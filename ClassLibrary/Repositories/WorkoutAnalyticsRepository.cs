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

public sealed class WorkoutAnalyticsRepository(AppDbContext dbContext) : IWorkoutAnalyticsRepository
{
    public async Task SaveWorkoutAsync(WorkoutLog log, CancellationToken cancellationToken = default)
    {
        await dbContext.WorkoutLogs.AddAsync(log, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> GetTotalWorkoutsAsync(int clientId, CancellationToken cancellationToken = default)
    {
        return await dbContext.WorkoutLogs
            .CountAsync(wl => wl.Client.ClientId == clientId, cancellationToken);
    }

    public async Task<IReadOnlyList<WorkoutLog>> GetWorkoutsInRangeAsync(
        int clientId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await dbContext.WorkoutLogs
            .AsNoTracking()
            .Where(wl => wl.Client.ClientId == clientId && wl.Date >= startDate && wl.Date <= endDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetTotalWorkoutCountAsync(int clientId, CancellationToken cancellationToken = default)
    {
        return await dbContext.WorkoutLogs
            .CountAsync(wl => wl.Client.ClientId == clientId, cancellationToken);
    }

    public async Task<(IReadOnlyList<WorkoutLog> Items, int TotalCount)> GetWorkoutHistoryPageAsync(
        int clientId, int skip, int take, CancellationToken cancellationToken = default)
    {
        var totalCount = await dbContext.WorkoutLogs
            .CountAsync(wl => wl.Client.ClientId == clientId, cancellationToken);

        var items = await dbContext.WorkoutLogs
            .AsNoTracking()
            .Where(wl => wl.Client.ClientId == clientId)
            .OrderByDescending(wl => wl.Date)
            .ThenByDescending(wl => wl.WorkoutLogId)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<WorkoutLog?> GetWorkoutLogWithSetsAsync(
        int clientId, int workoutLogId, CancellationToken cancellationToken = default)
    {
        return await dbContext.WorkoutLogs
            .AsNoTracking()
            .Include(wl => wl.Exercises)
            .FirstOrDefaultAsync(wl => wl.WorkoutLogId == workoutLogId && wl.Client.ClientId == clientId, cancellationToken);
    }
}