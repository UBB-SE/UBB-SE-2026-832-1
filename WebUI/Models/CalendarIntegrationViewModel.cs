using ClassLibrary.Models;

namespace WebUI.Models;

public class CalendarIntegrationViewModel
{
    public IReadOnlyList<WorkoutTemplate> AvailableWorkouts { get; set; } = new List<WorkoutTemplate>();
    public WorkoutTemplate? SelectedWorkout { get; set; }
    public int DurationWeeks { get; set; } = 4;
    public List<DaySelectionItem> SelectedDays { get; set; } = new();
    public string GeneratedIcsContent { get; set; } = string.Empty;
    public bool IsSuccess { get; set; }
    public string StatusMessage { get; set; } = string.Empty;
}

public class DaySelectionItem
{
    public int DayOfWeekIndex { get; set; }
    public string DayName { get; set; } = string.Empty;
    public bool IsSelected { get; set; }
}
