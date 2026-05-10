using System.Globalization;

namespace ClassLibrary.Models.Analytics;

public sealed class WorkoutSetRow
{
    public string ExerciseName { get; init; } = string.Empty;
    public int SetIndex { get; init; }
    public int? ActualReps { get; init; }
    public double? ActualWeight { get; init; }

    public string RepsDisplay =>
        ActualReps?.ToString(CultureInfo.InvariantCulture) ?? "—";

    public string WeightDisplay =>
        ActualWeight.HasValue
            ? ActualWeight.Value.ToString("F1", CultureInfo.InvariantCulture)
            : "—";
}
