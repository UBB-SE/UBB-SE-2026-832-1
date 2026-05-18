using System.Globalization;
using ClassLibrary.Proxies.Interfaces;

namespace WinUI.ViewModels;

public sealed class WorkoutLogSetSummary
{
    public int? Reps { get; }

    public double? Weight { get; }

    public string RepsDisplay => this.Reps?.ToString(CultureInfo.InvariantCulture) ?? "â€”";

    public string WeightDisplay => this.Weight.HasValue
        ? $"{this.Weight.Value.ToString("0.##", CultureInfo.InvariantCulture)} kg"
        : "â€”";

    public WorkoutLogSetSummary(int? reps, double? weight)
    {
        this.Reps = reps;
        this.Weight = weight;
    }
}

