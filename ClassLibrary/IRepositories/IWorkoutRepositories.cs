using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClassLibrary.Models;

namespace ClassLibrary.IRepositories;

public interface IWorkoutTemplateRepository
{
    Task<List<WorkoutTemplate>> GetAvailableWorkoutsAsync(int clientId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetAllExerciseNamesAsync(CancellationToken cancellationToken = default);
}

public interface IWorkoutLogRepository
{
    Task<List<WorkoutLog>> GetWorkoutHistoryAsync(int clientId, CancellationToken cancellationToken = default);
    Task<bool> UpdateWorkoutLogFeedbackAsync(int logId, int rating, string trainerNotes, CancellationToken cancellationToken = default);
}
