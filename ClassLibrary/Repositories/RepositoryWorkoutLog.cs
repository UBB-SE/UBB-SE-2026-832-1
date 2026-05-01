using ClassLibrary.Data;
using ClassLibrary.Models;
using ClassLibrary.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary.Repositories
{
    public class RepositoryWorkoutLog : IRepositoryWorkoutLog
    {
        private readonly AppDbContext dbContext;

        public RepositoryWorkoutLog(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<double> GetClientWeightAsync(int clientId, CancellationToken cancellationToken = default)
        {
            var client = await dbContext.Clients
                .FirstOrDefaultAsync(clientEntity => clientEntity.ClientId == clientId, cancellationToken);

            return client?.Weight ?? 75.0;
        }

        public async Task<bool> SaveWorkoutLogAsync(WorkoutLog workoutLog, CancellationToken cancellationToken = default)
        {
            await dbContext.WorkoutLogs.AddAsync(workoutLog, cancellationToken);
            return await dbContext.SaveChangesAsync(cancellationToken) > 0;
        }

        public async Task<IReadOnlyList<WorkoutLog>> GetWorkoutHistoryAsync(int clientId, CancellationToken cancellationToken = default)
        {
            return await dbContext.WorkoutLogs
                .Where(workoutLog => workoutLog.Client.ClientId == clientId)
                .OrderByDescending(workoutLog => workoutLog.Date)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> UpdateWorkoutLogFeedbackAsync(int workoutLogId, double rating, string notes, CancellationToken cancellationToken = default)
        {
            var workoutLog = await dbContext.WorkoutLogs
                .FirstOrDefaultAsync(logEntity => logEntity.WorkoutLogId == workoutLogId, cancellationToken);

            if (workoutLog == null)
                return false;

            workoutLog.Rating = rating;
            workoutLog.TrainerNotes = notes;

            return await dbContext.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}