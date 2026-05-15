using ClassLibrary.Models;

namespace ClassLibrary.Proxies.Interfaces;

public interface IActiveWorkoutProxy
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



