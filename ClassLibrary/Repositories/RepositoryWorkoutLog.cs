using ClassLibrary.Data;
using ClassLibrary.Models;
using ClassLibrary.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary.Repositories
{
    
    
    

    public class RepositoryWorkoutLog : IRepositoryWorkoutLog
    {
        private readonly AppDbContext _context;

        public RepositoryWorkoutLog(AppDbContext context)
        {
            _context = context;
        }

        public async Task<double> GetClientWeightAsync(int clientId)
        {
            var client = await _context.Clients
                .FirstOrDefaultAsync(c => c.ClientId == clientId);

            return client?.Weight ?? 75.0;
        }

        public async Task<bool> SaveWorkoutLogAsync(WorkoutLog log)
        {
            try
            {
                await _context.WorkoutLogs.AddAsync(log);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<WorkoutLog>> GetWorkoutHistoryAsync(int clientId)
        {
            return await _context.WorkoutLogs
                .Where(w => w.Client.ClientId == clientId)
                .OrderByDescending(w => w.Date)
                .ToListAsync();
        }

        public async Task<bool> UpdateWorkoutLogFeedbackAsync(int workoutLogId, double rating, string notes)
        {
            var log = await _context.WorkoutLogs
                .FirstOrDefaultAsync(w => w.WorkoutLogId == workoutLogId);

            if (log == null)
                return false;

            log.Rating = rating;
            log.TrainerNotes = notes;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}