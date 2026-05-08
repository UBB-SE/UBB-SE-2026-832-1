using System.Collections.ObjectModel;
using ClassLibrary.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WinUI.ViewModels;

public sealed partial class WorkoutLogItemViewModel : ObservableObject
{
    public int Id { get; }

    public string WorkoutName { get; }

    public DateTime Date { get; }

    public string DateDisplay { get; }

    public string TypeDisplay { get; }

    public string TotalDurationDisplay { get; }

    public ObservableCollection<WorkoutLogExerciseSummary> Exercises { get; } = new();

    [ObservableProperty]
    private bool isExpanded;

    public WorkoutLogItemViewModel(WorkoutLog workoutLog)
    {
        this.Id = workoutLog.WorkoutLogId;
        this.WorkoutName = string.IsNullOrWhiteSpace(workoutLog.WorkoutName) ? "Workout" : workoutLog.WorkoutName;
        this.Date = workoutLog.Date;
        this.DateDisplay = workoutLog.Date.ToString("yyyy-MM-dd");
        this.TypeDisplay = workoutLog.Type switch
        {
            WorkoutType.PREBUILT => "PRE-BUILT",
            WorkoutType.TRAINER_ASSIGNED => "TRAINER ASSIGNED",
            _ => "CUSTOM",
        };
        this.TotalDurationDisplay = workoutLog.Duration > TimeSpan.Zero
            ? workoutLog.Duration.ToString(@"hh\:mm")
            : "00:00";

        foreach (var exercise in workoutLog.Exercises)
        {
            this.Exercises.Add(new WorkoutLogExerciseSummary(exercise));
        }
    }
}
