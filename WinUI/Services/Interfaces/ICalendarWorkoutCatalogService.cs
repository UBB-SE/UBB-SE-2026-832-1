using ClassLibrary.Models;

namespace WinUI.Services.Interfaces;

public interface ICalendarWorkoutCatalogService
{
    Task<IReadOnlyList<WorkoutTemplate>> GetAvailableWorkoutsAsync(int clientId, TimeSpan timeout);

    IReadOnlyList<WorkoutTemplate> GetFallbackWorkouts(int clientId);
}
