namespace ClassLibrary.Models.Analytics;

public sealed class WorkoutSessionDetail
{
    public int WorkoutLogId { get; init; }
    public string WorkoutName { get; init; } = string.Empty;
    public DateTime LogDate { get; init; }
    public int DurationSeconds { get; init; }
    public int TotalCaloriesBurned { get; init; }
    public string IntensityTag { get; init; } = string.Empty;
    public IReadOnlyList<WorkoutSetRow> Sets { get; init; } = Array.Empty<WorkoutSetRow>();
    public IReadOnlyList<ExerciseCalorieInfo> ExerciseCalories { get; init; } = Array.Empty<ExerciseCalorieInfo>();
}
