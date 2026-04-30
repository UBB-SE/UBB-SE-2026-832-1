using ClassLibrary.Data;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary.Repositories;

public sealed class RepositoryWorkoutLog : IRepositoryWorkoutLog
{
    private readonly AppDbContext databaseContext;

    public RepositoryWorkoutLog(AppDbContext databaseContext)
    {
        this.databaseContext = databaseContext;
    }

    public async Task<IReadOnlyList<WorkoutLog>> GetWorkoutHistoryAsync(int clientId, CancellationToken cancellationToken = default)
    {
        return await this.databaseContext.WorkoutLogs
            .AsNoTracking()
            .Where(workoutLog => workoutLog.Client.ClientId == clientId)
            .Include(workoutLog => workoutLog.Client)
            .Include(workoutLog => workoutLog.Exercises)
                .ThenInclude(loggedExercise => loggedExercise.Sets)
            .OrderByDescending(workoutLog => workoutLog.Date)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> SaveWorkoutLogAsync(WorkoutLog log, CancellationToken cancellationToken = default)
    {
        if (log == null)
        {
            return false;
        }

        await this.databaseContext.WorkoutLogs.AddAsync(log, cancellationToken);
        return await this.databaseContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> UpdateWorkoutLogAsync(WorkoutLog log, CancellationToken cancellationToken = default)
    {
        if (log == null)
        {
            return false;
        }

        var existing = await this.databaseContext.WorkoutLogs
            .FirstOrDefaultAsync(workoutLog => workoutLog.WorkoutLogId == log.WorkoutLogId, cancellationToken);

        if (existing == null)
        {
            return false;
        }

        this.databaseContext.Entry(existing).CurrentValues.SetValues(log);
        await this.databaseContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<double> GetClientWeightAsync(int clientId, CancellationToken cancellationToken = default)
    {
        var client = await this.databaseContext.Clients
            .AsNoTracking()
            .FirstOrDefaultAsync(client => client.ClientId == clientId, cancellationToken);

        return client?.Weight ?? 0;
    }
}
