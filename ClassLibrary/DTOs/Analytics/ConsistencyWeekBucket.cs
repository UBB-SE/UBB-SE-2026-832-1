namespace ClassLibrary.DTOs.Analytics;

public sealed class ConsistencyWeekBucket
{
    public DateOnly WeekStart { get; init; }

    public int WorkoutCount { get; init; }
}
