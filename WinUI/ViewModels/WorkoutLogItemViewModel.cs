using System.Collections.ObjectModel;
using ClassLibrary.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WinUI.ViewModels;

public sealed partial class WorkoutLogItemViewModel : ObservableObject
{
    private WorkoutLog log;

    public int Id { get; }

    public string WorkoutName { get; }

    public DateTime Date { get; }

    public string DateDisplay { get; }

    public string TypeDisplay { get; }

    public string TotalDurationDisplay { get; }

    public ObservableCollection<WorkoutLogExerciseSummary> Exercises { get; } = new();

    [ObservableProperty]
    private bool isExpanded;

    [ObservableProperty]
    private bool isEditMode;

    public WorkoutLogItemViewModel(WorkoutLog workoutLog)
    {
        this.log = workoutLog;
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
        this.LoadExercisesFromLog(workoutLog);
    }

    public void EnterEditMode() => this.IsEditMode = true;

    public void CancelEditMode()
    {
        this.LoadExercisesFromLog(this.log);
        this.IsEditMode = false;
    }

    public void CommitEditMode()
    {
        this.log.Exercises = this.BuildUpdatedExerciseCollection();
        this.LoadExercisesFromLog(this.log);
        this.IsEditMode = false;
    }

    public WorkoutLog BuildUpdatedWorkoutLog()
    {
        var updatedExercises = this.BuildUpdatedExerciseCollection();
        return new WorkoutLog
        {
            WorkoutLogId = this.log.WorkoutLogId,
            Client = this.log.Client,
            WorkoutName = this.log.WorkoutName,
            Date = this.log.Date,
            Duration = this.log.Duration,
            SourceTemplateId = this.log.SourceTemplateId,
            Type = this.log.Type,
            Exercises = updatedExercises,
            TotalCaloriesBurned = this.log.TotalCaloriesBurned,
            AverageMetabolicEquivalent = this.log.AverageMetabolicEquivalent,
            IntensityTag = this.log.IntensityTag,
            Rating = this.log.Rating,
            TrainerNotes = this.log.TrainerNotes,
        };
    }

    private List<LoggedExercise> BuildUpdatedExerciseCollection()
    {
        return this.Exercises
            .Select(e => e.ToLoggedExercise(this.log.WorkoutLogId))
            .ToList();
    }

    private void LoadExercisesFromLog(WorkoutLog workoutLog)
    {
        this.Exercises.Clear();
        foreach (var exercise in workoutLog.Exercises)
        {
            this.Exercises.Add(new WorkoutLogExerciseSummary(exercise));
        }
    }
}
