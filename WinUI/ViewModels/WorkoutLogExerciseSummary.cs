using System.Collections.ObjectModel;
using ClassLibrary.Models;
using ClassLibrary.Proxies.Interfaces;

namespace WinUI.ViewModels;

public sealed class WorkoutLogExerciseSummary
{
    public string ExerciseName { get; }

    public bool IsSystemAdjusted { get; }

    public string TooltipText { get; }

    public ObservableCollection<WorkoutLogSetSummary> Sets { get; } = new();

    public int NumberOfSets => this.Sets.Count;

    public string RepsDisplay => this.Sets.Count > 0
        ? string.Join(" / ", this.Sets.Select(s => s.RepsDisplay))
        : "â€”";

    public string WeightDisplay => this.Sets.Count > 0
        ? string.Join(" / ", this.Sets.Select(s => s.WeightDisplay))
        : "â€”";

    public WorkoutLogExerciseSummary(LoggedExercise exercise)
    {
        this.ExerciseName = exercise.ExerciseName;
        this.IsSystemAdjusted = exercise.IsSystemAdjusted;
        this.TooltipText = !string.IsNullOrWhiteSpace(exercise.AdjustmentNote)
            ? exercise.AdjustmentNote
            : $"Performance: {exercise.PerformanceRatio * 100:F0}% of target reps achieved.";

        foreach (var set in exercise.Sets.OrderBy(s => s.SetIndex))
        {
            this.Sets.Add(new WorkoutLogSetSummary(set.ActualReps, set.ActualWeight));
        }
    }
}

