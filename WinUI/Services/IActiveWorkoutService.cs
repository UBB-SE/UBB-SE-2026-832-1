using ClassLibrary.Models;

namespace WinUI.Services;

public interface IActiveWorkoutService
{
    Task<IReadOnlyList<WorkoutTemplate>> GetAvailableWorkoutsForClient(int clientId);

    Task<IReadOnlyList<WorkoutTemplate>> GetCustomAndTrainerAssignedWorkoutsForClient(int clientId);

    Task<WorkoutTemplate?> FindWorkoutTemplateById(int clientId, int? id);

    Task<IDictionary<string, double>> GetPreviousBestWeightsAsync(int clientId);

    Task<bool> SaveSetAsync(WorkoutLog workoutLog, LoggedSet set);

    Task<bool> FinalizeWorkoutAsync(WorkoutLog workoutLog);

    Task<IReadOnlyList<Notification>> GetNotifications(int clientId);

    Task ConfirmDeload(Notification notification);
}
