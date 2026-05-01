using ClassLibrary.Data;
using ClassLibrary.Models;
using ClassLibrary.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary.Repositories;

public class RepositoryWorkoutLog : IRepositoryWorkoutLog
{
    private readonly AppDbContext databaseContext;

    private const double DefaultClientWeight = 75.0;

    public RepositoryWorkoutLog(AppDbContext databaseContext)
    {
        this.databaseContext = databaseContext;
    }

    public async Task<double> GetClientWeightAsync(int clientId)
    {
        var client = await databaseContext.Clients
            .FirstOrDefaultAsync(clientEntity => clientEntity.ClientId == clientId);

        return client?.Weight ?? DefaultClientWeight;
    }

    public async Task<bool> SaveWorkoutLogAsync(WorkoutLog workoutLog)
    {
        await databaseContext.WorkoutLogs.AddAsync(workoutLog);
        return await databaseContext.SaveChangesAsync() > 0;
    }

    public async Task<IReadOnlyList<WorkoutLog>> GetWorkoutHistoryAsync(int clientId)
    {
        return await databaseContext.WorkoutLogs
            .Where(workoutLog => workoutLog.Client.ClientId == clientId)
            .OrderByDescending(workoutLog => workoutLog.Date)
            .ToListAsync();
    }

    public async Task<bool> UpdateWorkoutLogFeedbackAsync(int workoutLogId, double rating, string notes)
    {
        var workoutLog = await databaseContext.WorkoutLogs
            .FirstOrDefaultAsync(logEntity => logEntity.WorkoutLogId == workoutLogId);

        if (workoutLog == null)
            return false;

        workoutLog.Rating = rating;
        workoutLog.TrainerNotes = notes;

        return await databaseContext.SaveChangesAsync() > 0;
    }
}