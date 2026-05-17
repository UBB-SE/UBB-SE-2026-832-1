using ClassLibrary.DTOs.Analytics;

namespace WebUI.Models.ClientDashboard;

public sealed class WorkoutHistoryItemViewModel
{
    public int WorkoutLogId { get; init; }

    public string Title { get; init; } = string.Empty;

    public string DateLine { get; init; } = string.Empty;

    public string DurationLine { get; init; } = string.Empty;

    public string IntensityTag { get; init; } = string.Empty;

    public static WorkoutHistoryItemViewModel FromRow(WorkoutHistoryRow row) =>
        new()
        {
            WorkoutLogId = row.Id,
            Title = row.WorkoutName,
            DateLine = row.LogDate.ToString("MMM dd, yyyy"),
            DurationLine = FormatDuration(row.DurationSeconds),
            IntensityTag = row.IntensityTag,
        };

    private static string FormatDuration(int seconds)
    {
        var span = TimeSpan.FromSeconds(seconds);
        return span.Hours > 0 ? $"{span.Hours}h {span.Minutes}m" : $"{span.Minutes}m";
    }
}
