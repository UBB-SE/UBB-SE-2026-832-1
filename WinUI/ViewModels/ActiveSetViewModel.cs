using CommunityToolkit.Mvvm.ComponentModel;

namespace WinUI.ViewModels;

public sealed partial class ActiveSetViewModel : ObservableObject
{
    public string ExerciseName { get; set; } = string.Empty;

    public int SetIndex { get; set; }

    public int? TargetReps { get; set; }

    public double? TargetWeight { get; set; }

    [ObservableProperty]
    private int? actualReps;

    [ObservableProperty]
    private double? actualWeight;

    [ObservableProperty]
    private bool isCompleted;

    [ObservableProperty]
    private bool isFocused;

    public Action<ActiveSetViewModel>? AutoSaveHandler { get; set; }

    public double ActualRepsValue
    {
        get => this.ActualReps.HasValue ? this.ActualReps.Value : double.NaN;
        set => this.ActualReps = double.IsNaN(value) ? null : (int)Math.Round(value);
    }

    public double ActualWeightValue
    {
        get => this.ActualWeight ?? double.NaN;
        set => this.ActualWeight = double.IsNaN(value) ? null : value;
    }

    partial void OnActualRepsChanged(int? value)
    {
        OnPropertyChanged(nameof(ActualRepsValue));
        TryAutoSave();
    }

    partial void OnActualWeightChanged(double? value)
    {
        OnPropertyChanged(nameof(ActualWeightValue));
        TryAutoSave();
    }

    private void TryAutoSave()
    {
        if (this.IsCompleted)
        {
            return;
        }

        if (!this.ActualReps.HasValue || !this.ActualWeight.HasValue)
        {
            return;
        }

        this.AutoSaveHandler?.Invoke(this);
    }
}
