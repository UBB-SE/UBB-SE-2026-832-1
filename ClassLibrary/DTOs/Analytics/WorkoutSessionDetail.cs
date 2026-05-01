namespace ClassLibrary.DTOs.Analytics;

public sealed class WorkoutSessionDetail
{
    public int WorkoutLogId { get; init; }
    public string WorkoutName { get; init; } = string.Empty;
    public DateTime LogDate { get; init; }
    public int DurationSeconds { get; init; }
    public int TotalCaloriesBurned { get; init; }
    public string IntensityTag { get; init; } = string.Empty;
    public IReadOnlyList<WorkoutSetRow> Sets { get; init; } = [];
    public IReadOnlyList<ExerciseCalorieInfo> ExerciseCalories { get; init; } = [];
}

public sealed class ExerciseCalorieInfo
{
    public string ExerciseName { get; init; } = string.Empty;
    public int CaloriesBurned { get; init; }
}

public sealed class WorkoutSetRow
{
    public string ExerciseName { get; init; } = string.Empty;
    public int SetIndex { get; init; }
    public int? ActualReps { get; init; }
    public double? ActualWeight { get; init; }
}
