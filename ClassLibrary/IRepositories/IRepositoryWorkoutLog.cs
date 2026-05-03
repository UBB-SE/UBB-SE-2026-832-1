using ClassLibrary.Models;

namespace ClassLibrary.IRepositories;

public interface IRepositoryWorkoutLog
{
    Task<double> GetClientWeightAsync(int clientId);

    Task<bool> SaveWorkoutLogAsync(WorkoutLog workoutLog);

    Task<IReadOnlyList<WorkoutLog>> GetWorkoutHistoryAsync(int clientId);

    Task<bool> UpdateWorkoutLogFeedbackAsync(int workoutLogId, double rating, string notes);
}