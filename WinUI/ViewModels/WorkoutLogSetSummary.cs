using System.Globalization;

namespace WinUI.ViewModels;

public sealed class WorkoutLogSetSummary
{
    public int? Reps { get; }

    public double? Weight { get; }

    public string RepsDisplay => this.Reps?.ToString(CultureInfo.InvariantCulture) ?? "—";

    public string WeightDisplay => this.Weight.HasValue
        ? $"{this.Weight.Value.ToString("0.##", CultureInfo.InvariantCulture)} kg"
        : "—";

    public WorkoutLogSetSummary(int? reps, double? weight)
    {
        this.Reps = reps;
        this.Weight = weight;
    }
}
