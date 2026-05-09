using ClassLibrary.Models;

namespace WinUI.Services.Interfaces;

public interface ICalendarExportService
{
    string GenerateCalendar(WorkoutTemplate workoutTemplate, int durationWeeks, int[] selectedDays, DateTime? startDate = null);

    Task<string?> SaveCalendarToDownloadsAsync(string calendarContent, string? workoutName);
}
