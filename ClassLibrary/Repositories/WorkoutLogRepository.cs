using ClassLibrary.Data;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary.Repositories;

public sealed class WorkoutLogRepository : IWorkoutLogRepository
{
    private readonly AppDbContext databaseContext;

    public WorkoutLogRepository(AppDbContext databaseContext)
    {
        this.databaseContext = databaseContext;
    }

    public async Task<IReadOnlyList<WorkoutLog>> GetWorkoutHistoryAsync(int clientId)
    {
        return await this.databaseContext.WorkoutLogs
            .AsNoTracking()
            .Where(workoutLog => workoutLog.Client.ClientId == clientId)
            .Include(workoutLog => workoutLog.Client)
            .Include(workoutLog => workoutLog.Exercises)
                .ThenInclude(loggedExercise => loggedExercise.Sets)
            .OrderByDescending(workoutLog => workoutLog.Date)
            .ToListAsync();
    }

    public async Task SaveWorkoutLogAsync(WorkoutLog log)
    {
        if (log == null)
        {
            throw new ArgumentNullException(nameof(log));
        }
        if (log.Client != null)
        {
            // 1. Attach the client so EF knows it already exists
            this.databaseContext.Clients.Attach(log.Client);

            // 2. Explicitly mark it as Unchanged so no INSERT/UPDATE happens for the Client
            this.databaseContext.Entry(log.Client).State = EntityState.Unchanged;

            // 3. If there is a User object inside Client, mark that Unchanged too
            if (log.Client.User != null)
            {
                this.databaseContext.Entry(log.Client.User).State = EntityState.Unchanged;
            }
        }

        await this.databaseContext.WorkoutLogs.AddAsync(log);
        await this.databaseContext.SaveChangesAsync();
    }

    public async Task UpdateWorkoutLogAsync(WorkoutLog log)
    {
        if (log == null)
        {
            throw new ArgumentNullException(nameof(log));
        }

        var existing = await this.databaseContext.WorkoutLogs
            .FirstOrDefaultAsync(workoutLog => workoutLog.WorkoutLogId == log.WorkoutLogId);

        if (existing == null)
        {
            throw new KeyNotFoundException($"Workout log with ID {log.WorkoutLogId} not found.");
        }

        this.databaseContext.Entry(existing).CurrentValues.SetValues(log);
        await this.databaseContext.SaveChangesAsync();
    }

    public async Task<double> GetClientWeightAsync(int clientId)
    {
        var client = await this.databaseContext.Clients
            .AsNoTracking()
            .FirstOrDefaultAsync(client => client.ClientId == clientId);

        return client?.Weight ?? 0;
    }
}
