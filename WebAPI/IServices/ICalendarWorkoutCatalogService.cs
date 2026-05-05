using ClassLibrary.DTOs;

namespace WebAPI.IServices;

public interface ICalendarWorkoutCatalogService
{
    Task<CalendarWorkoutCatalogResponseDataTransferObject> GetAvailableWorkoutsAsync(int clientId, TimeSpan timeout);

    CalendarWorkoutCatalogResponseDataTransferObject GetFallbackWorkouts(int clientId);
}
