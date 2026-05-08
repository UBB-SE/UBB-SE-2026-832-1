using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WinUI.ViewModels;

public sealed partial class WorkoutLogSetEditorViewModel : ObservableObject
{
    public int SetNumber { get; init; }

    [ObservableProperty]
    private int? reps;

    [ObservableProperty]
    private double? weight;

    public double RepsInput
    {
        get => this.Reps.HasValue ? this.Reps.Value : double.NaN;
        set => this.Reps = double.IsNaN(value) ? null : (int)Math.Round(value);
    }

    public double WeightInput
    {
        get => this.Weight ?? double.NaN;
        set => this.Weight = double.IsNaN(value) ? null : value;
    }

    public string RepsDisplay => this.Reps?.ToString(CultureInfo.InvariantCulture) ?? "—";

    public string WeightDisplay => this.Weight.HasValue
        ? $"{this.Weight.Value.ToString("0.##", CultureInfo.InvariantCulture)} kg"
        : "—";

    partial void OnRepsChanged(int? value)
    {
        OnPropertyChanged(nameof(RepsInput));
        OnPropertyChanged(nameof(RepsDisplay));
    }

    partial void OnWeightChanged(double? value)
    {
        OnPropertyChanged(nameof(WeightInput));
        OnPropertyChanged(nameof(WeightDisplay));
    }
}
