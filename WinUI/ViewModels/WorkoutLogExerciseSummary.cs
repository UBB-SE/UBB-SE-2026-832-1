using System.Collections.ObjectModel;
using ClassLibrary.Models;

namespace WinUI.ViewModels;

public sealed class WorkoutLogExerciseSummary
{
    public string ExerciseName { get; }

    public bool IsSystemAdjusted { get; }

    public string TooltipText { get; }

    public ObservableCollection<WorkoutLogSetEditorViewModel> Sets { get; } = new();

    public int NumberOfSets => this.Sets.Count;

    public string RepsDisplay => this.Sets.Count > 0
        ? string.Join(" / ", this.Sets.Select(s => s.RepsDisplay))
        : "—";

    public string WeightDisplay => this.Sets.Count > 0
        ? string.Join(" / ", this.Sets.Select(s => s.WeightDisplay))
        : "—";

    public WorkoutLogExerciseSummary(LoggedExercise exercise)
    {
        this.ExerciseName = exercise.ExerciseName;
        this.IsSystemAdjusted = exercise.IsSystemAdjusted;
        this.TooltipText = !string.IsNullOrWhiteSpace(exercise.AdjustmentNote)
            ? exercise.AdjustmentNote
            : $"Performance: {exercise.PerformanceRatio * 100:F0}% of target reps achieved.";

        int index = 1;
        foreach (var set in exercise.Sets.OrderBy(s => s.SetIndex))
        {
            this.Sets.Add(new WorkoutLogSetEditorViewModel
            {
                SetNumber = index++,
                Reps = set.ActualReps,
                Weight = set.ActualWeight,
            });
        }
    }

    public LoggedExercise ToLoggedExercise(int workoutLogId)
    {
        return new LoggedExercise
        {
            ExerciseName = this.ExerciseName,
            IsSystemAdjusted = this.IsSystemAdjusted,
            AdjustmentNote = this.TooltipText,
            WorkoutLog = null!,
            Sets = this.Sets.Select((s, i) => new LoggedSet
            {
                ExerciseName = this.ExerciseName,
                SetIndex = i + 1,
                SetNumber = i + 1,
                ActualReps = s.Reps,
                ActualWeight = s.Weight,
                WorkoutLog = null!,
            }).ToList(),
        };
    }
}
