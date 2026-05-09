namespace ClassLibrary.DTOs;

public class DashboardSummaryDto
{
    public int TotalWorkouts { get; set; }
    public double TotalActiveTimeLastSevenDays { get; set; }
    public string PreferredWorkoutName { get; set; } = string.Empty;
}