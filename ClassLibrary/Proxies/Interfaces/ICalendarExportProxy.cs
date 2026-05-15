using ClassLibrary.Models;

namespace ClassLibrary.Proxies.Interfaces;

public interface ICalendarExportProxy
{
    string GenerateCalendar(WorkoutTemplate workoutTemplate, int durationWeeks, int[] selectedDays, DateTime? startDate = null);

    Task<string?> SaveCalendarToDownloadsAsync(string calendarContent, string? workoutName);
}



