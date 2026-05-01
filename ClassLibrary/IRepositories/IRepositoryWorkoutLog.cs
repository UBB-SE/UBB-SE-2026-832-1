using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClassLibrary.Models;

namespace ClassLibrary.IRepositories
{
    public interface IRepositoryWorkoutLog
    {
        Task<double> GetClientWeightAsync(int clientId, CancellationToken cancellationToken = default);

        Task<bool> SaveWorkoutLogAsync(WorkoutLog workoutLog, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<WorkoutLog>> GetWorkoutHistoryAsync(int clientId, CancellationToken cancellationToken = default);

        Task<bool> UpdateWorkoutLogFeedbackAsync(int workoutLogId, double rating, string notes, CancellationToken cancellationToken = default);
    }
}