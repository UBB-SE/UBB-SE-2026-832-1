using ClassLibrary.Models;

namespace ClassLibrary.Proxies.Interfaces;

public interface ICalendarWorkoutCatalogProxy
{
    Task<IReadOnlyList<WorkoutTemplate>> GetAvailableWorkoutsAsync(int clientId, TimeSpan timeout);

    IReadOnlyList<WorkoutTemplate> GetFallbackWorkouts(int clientId);
}



