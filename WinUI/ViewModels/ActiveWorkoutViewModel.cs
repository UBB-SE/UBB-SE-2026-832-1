using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using ClassLibrary.DTOs;
using WinUI.Services;
using ClassLibrary.Models;

namespace WinUI.ViewModels;

public partial class ActiveWorkoutViewModel : ObservableObject
{
    private const int DefaultRestTimeSeconds = 90;
    private const int RestTimerIntervalMilliseconds = 1000;
    private const int HourInSeconds = 3600;

    private readonly IActiveWorkoutService activeWorkoutService;
    private readonly INavigationService navigationService;
    private readonly WorkoutUiState workoutUiState;

    private WorkoutLog activeLog;
    private ActiveSetViewModel? currentPendingSet;
    private System.Timers.Timer? restTimer;
    private DispatcherTimer? elapsedTimer;
    private TimeSpan elapsedWorkout;

    public ActiveWorkoutViewModel(
        IActiveWorkoutService activeWorkoutService,
        INavigationService navigationService,
        WorkoutUiState workoutUiState)
    {
        this.activeWorkoutService = activeWorkoutService;
        this.navigationService = navigationService;
        this.workoutUiState = workoutUiState;
        this.activeLog = new WorkoutLog { Date = DateTime.Now };
    }

    [ObservableProperty]
    public partial int RestTimeRemaining { get; set; }

    [ObservableProperty]
    public partial bool IsResting { get; set; }

    [ObservableProperty]
    public partial string WorkoutElapsedDisplay { get; set; } = "00:00";

    [ObservableProperty]
    public partial string WorkoutSessionTitle { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string CurrentExerciseName { get; set; } = string.Empty;

    [ObservableProperty]
    public partial int? CurrentTargetReps { get; set; }

    [ObservableProperty]
    public partial int CurrentSetNumber { get; set; }

    [ObservableProperty]
    public partial double CurrentSetRepsInput { get; set; } = double.NaN;

    [ObservableProperty]
    public partial double CurrentSetWeightInput { get; set; } = double.NaN;

    [ObservableProperty]
    public partial ObservableCollection<WorkoutTemplate> AvailableWorkouts { get; set; } = new();

    [ObservableProperty]
    public partial ObservableCollection<WorkoutTemplate> CustomWorkouts { get; set; } = new();

    [ObservableProperty]
    public partial bool HasCustomWorkouts { get; set; }

    [ObservableProperty]
    public partial WorkoutTemplate? SelectedTemplate { get; set; }

    [ObservableProperty]
    public partial bool IsLoadingWorkouts { get; set; }

    [ObservableProperty]
    public partial string SelectedGoal { get; set; } = string.Empty;

    [ObservableProperty]
    public partial ObservableCollection<ActiveExerciseViewModel> ExerciseRows { get; set; } = new();

    [ObservableProperty]
    public partial bool IsWorkoutStarted { get; set; }

    [ObservableProperty]
    public partial bool IsFinishing { get; set; }

    [ObservableProperty]
    public partial string ErrorMessage { get; set; } = string.Empty;

    public void StartRestTimer(int seconds = DefaultRestTimeSeconds)
    {
        if (seconds <= 0)
        {
            this.IsResting = false;
            this.RestTimeRemaining = 0;
            this.restTimer?.Stop();
            return;
        }

        var dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        if (dispatcherQueue is null)
        {
            return;
        }

        this.RestTimeRemaining = seconds;
        this.IsResting = true;

        this.restTimer?.Stop();
        this.restTimer = new System.Timers.Timer(RestTimerIntervalMilliseconds);

        this.restTimer.Elapsed += (_, _) =>
        {
            dispatcherQueue.TryEnqueue(() =>
            {
                if (this.RestTimeRemaining > 0)
                {
                    this.RestTimeRemaining--;
                }
                else
                {
                    this.restTimer?.Stop();
                    this.IsResting = false;
                }
            });
        };

        this.restTimer?.Start();
    }

    [RelayCommand]
    private void SetRestTime(string? timeText)
    {
        if (string.IsNullOrWhiteSpace(timeText))
        {
            return;
        }

        if (int.TryParse(timeText, out int seconds))
        {
            if (seconds < 0)
            {
                seconds = 0;
            }

            if (seconds > HourInSeconds)
            {
                seconds = HourInSeconds;
            }

            this.StartRestTimer(seconds);
        }
    }

    partial void OnIsWorkoutStartedChanged(bool value)
    {
        if (value)
        {
            elapsedWorkout = TimeSpan.Zero;
            WorkoutElapsedDisplay = "00:00";
            StartWorkoutElapsedTimer();
        }
        else
        {
            StopWorkoutElapsedTimer();
        }
    }

    private void StartWorkoutElapsedTimer()
    {
        this.StopWorkoutElapsedTimer();
        this.elapsedTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        this.elapsedTimer.Tick += (_, _) =>
        {
            this.elapsedWorkout = this.elapsedWorkout.Add(TimeSpan.FromSeconds(1));
            this.WorkoutElapsedDisplay = this.elapsedWorkout.ToString(@"mm\:ss");
        };
        this.elapsedTimer.Start();
    }

    private void StopWorkoutElapsedTimer()
    {
        if (this.elapsedTimer is null)
        {
            return;
        }

        this.elapsedTimer.Stop();
        this.elapsedTimer = null;
    }

    private static string? BuildProgressionHeadsUp(WorkoutLog log)
    {
        var lines = log.Exercises
            .Where(e => e.IsSystemAdjusted || !string.IsNullOrWhiteSpace(e.AdjustmentNote))
            .Select(e => string.IsNullOrWhiteSpace(e.AdjustmentNote)
                ? $"{e.ExerciseName}: targets were adjusted for next time."
                : $"{e.ExerciseName}: {e.AdjustmentNote}")
            .ToList();
        return lines.Count == 0 ? null : string.Join("\n", lines);
    }

    public void LoadCustomWorkouts(int clientId)
    {
        var customAndTrainerAssignedWorkouts = this.activeWorkoutService.GetCustomAndTrainerAssignedWorkoutsForClient(clientId).Result;
        this.CustomWorkouts.Clear();
        for (int workoutIndex = 0; workoutIndex < customAndTrainerAssignedWorkouts.Count; workoutIndex++)
        {
            this.CustomWorkouts.Add(customAndTrainerAssignedWorkouts[workoutIndex]);
        }

        this.HasCustomWorkouts = this.CustomWorkouts.Count > 0;
    }

    [RelayCommand]
    private void SelectCustomWorkout(WorkoutTemplate template)
    {
        this.SelectedTemplate = null;
        this.SelectedTemplate = template;
    }

    [RelayCommand]
    private async void ApplyTargetGoals(int clientId)
    {
        if (string.IsNullOrEmpty(this.SelectedGoal))
        {
            return;
        }

        var selectedGoalNames = new List<string> { this.SelectedGoal };

        try
        {
            this.IsLoadingWorkouts = true;

            var allWorkouts = await this.activeWorkoutService.GetAvailableWorkoutsForClient(clientId);
            var selected = allWorkouts
                .Where(w => selectedGoalNames.Contains(w.Name))
                .ToList();

            if (selected.Count == 0)
            {
                return;
            }

            this.AvailableWorkouts.Clear();
            foreach (var w in selected)
            {
                this.AvailableWorkouts.Add(w);
            }

            this.activeLog = new WorkoutLog
            {
                WorkoutName = string.Join(" + ", selected.Select(t => t.Name)),
                SourceTemplateId = selected[0].WorkoutTemplateId,
                Type = selected[0].Type,
                Date = DateTime.Now,
            };

            this.ExerciseRows.Clear();
            foreach (var template in selected)
            {
                foreach (var exercise in template.GetExercises())
                {
                    this.ExerciseRows.Add(new ActiveExerciseViewModel(exercise, this.SaveSet));
                }
            }

            this.UpdateCurrentSetDisplay();
            this.WorkoutSessionTitle = this.activeLog.WorkoutName;
            this.IsWorkoutStarted = true;
        }
        catch (Exception)
        {
        }
        finally
        {
            this.IsLoadingWorkouts = false;
        }
    }

    partial void OnSelectedTemplateChanged(WorkoutTemplate? value)
    {
        if (value == null)
        {
            return;
        }

        activeLog = new WorkoutLog
        {
            WorkoutName = value.Name,
            SourceTemplateId = value.WorkoutTemplateId,
            Type = value.Type,
            Date = DateTime.Now
        };

        ExerciseRows.Clear();
        foreach (var exercise in value.GetExercises())
        {
            ExerciseRows.Add(new ActiveExerciseViewModel(exercise, SaveSet));
        }

        UpdateCurrentSetDisplay();
        WorkoutSessionTitle = value.Name;
        IsWorkoutStarted = true;
    }

    [RelayCommand]
    private void SaveSet(ActiveSetViewModel setViewModel)
    {
        if (setViewModel == null || !this.IsWorkoutStarted)
        {
            return;
        }

        this.ErrorMessage = string.Empty;

        var set = new LoggedSet
        {
            ExerciseName = setViewModel.ExerciseName,
            SetIndex = setViewModel.SetIndex,
            ActualReps = setViewModel.ActualReps,
            ActualWeight = setViewModel.ActualWeight,
            TargetReps = setViewModel.TargetReps,
            TargetWeight = null,
        };

        var dto = new LoggedExerciseDataTransferObject
        {
            ExerciseName = set.ExerciseName,
            Sets = set.SetIndex,
            ActualReps = set.ActualReps ?? 0,
            ActualWeight = set.ActualWeight ?? 0,
        };

        var workoutDto = new WorkoutLogDataTransferObject
        {
            WorkoutLogId = this.activeLog.WorkoutLogId,
            WorkoutName = this.activeLog.WorkoutName,
            Date = this.activeLog.Date,
            Duration = this.activeLog.Duration,
            SourceTemplateId = this.activeLog.SourceTemplateId,
            Type = this.activeLog.Type.ToString(),
            Exercises = new List<LoggedExerciseDataTransferObject> { dto },
            TotalCaloriesBurned = this.activeLog.TotalCaloriesBurned,
            AverageMetabolicEquivalent = this.activeLog.AverageMetabolicEquivalent,
            IntensityTag = this.activeLog.IntensityTag,
            Rating = this.activeLog.Rating,
            TrainerNotes = this.activeLog.TrainerNotes,
        };

        var isSaved = this.activeWorkoutService.SaveSetAsync(workoutDto).Result;
        if (!isSaved)
        {
            this.ErrorMessage = "Failed to save set. Please try again.";
            return;
        }

        setViewModel.IsCompleted = true;

        this.FocusNextSet(setViewModel);
        this.UpdateCurrentSetDisplay();
    }

    [RelayCommand]
    private async void FinishWorkout(int clientId)
    {
        if (!this.IsWorkoutStarted)
        {
            return;
        }

        try
        {
            this.IsFinishing = true;
            this.ErrorMessage = string.Empty;

            this.activeLog.Client.ClientId = clientId;
            this.activeLog.Duration = this.elapsedWorkout;

            var workoutDto = new WorkoutLogDataTransferObject
            {
                WorkoutLogId = this.activeLog.WorkoutLogId,
                WorkoutName = this.activeLog.WorkoutName,
                Date = this.activeLog.Date,
                Duration = this.activeLog.Duration,
                SourceTemplateId = this.activeLog.SourceTemplateId,
                Type = this.activeLog.Type.ToString(),
                Exercises = this.activeLog.Exercises.Select(e => new LoggedExerciseDataTransferObject
                {
                    ExerciseName = e.ExerciseName,
                    Sets = e.Sets.Count,
                    ActualReps = e.Sets.LastOrDefault()?.ActualReps ?? 0,
                    ActualWeight = e.Sets.LastOrDefault()?.ActualWeight ?? 0,
                }).ToList(),
                TotalCaloriesBurned = this.activeLog.TotalCaloriesBurned,
                AverageMetabolicEquivalent = this.activeLog.AverageMetabolicEquivalent,
                IntensityTag = this.activeLog.IntensityTag,
                Rating = this.activeLog.Rating,
                TrainerNotes = this.activeLog.TrainerNotes,
            };

            var success = await this.activeWorkoutService.FinalizeWorkoutAsync(workoutDto);

            if (success)
            {
                this.LastCompletedLog = this.activeLog;
                this.workoutUiState.ProgressionHeadsUp = BuildProgressionHeadsUp(this.activeLog);
                this.IsWorkoutStarted = false;
                this.ExerciseRows.Clear();
                this.activeLog = new WorkoutLog { Date = DateTime.Now };
                this.WorkoutSessionTitle = string.Empty;
                this.CurrentExerciseName = string.Empty;
                this.CurrentTargetReps = null;
                this.CurrentSetNumber = 0;
                this.CurrentSetRepsInput = double.NaN;
                this.CurrentSetWeightInput = double.NaN;

                this.navigationService.NavigateToClientDashboard(true);
            }
            else
            {
                this.ErrorMessage = "Failed to save workout. Please try again.";
            }
        }
        catch (Exception)
        {
            this.ErrorMessage = "Error finishing workout";
        }
        finally
        {
            this.IsFinishing = false;
        }
    }

    public WorkoutLog? LastCompletedLog { get; private set; }

    [RelayCommand]
    private async void RepeatWorkout(int clientId)
    {
        if (this.LastCompletedLog == null)
        {
            return;
        }

        var template = await this.activeWorkoutService.FindWorkoutTemplateById(clientId, this.LastCompletedLog.SourceTemplateId);

        if (template == null)
        {
            return;
        }

        this.SelectedTemplate = template.ToModel();
    }

    [ObservableProperty]
    public partial ObservableCollection<Models.Notification> Notifications { get; set; } = new();

    [RelayCommand]
    private async void LoadNotifications(int clientId)
    {
        this.Notifications.Clear();
        var list = await this.activeWorkoutService.GetNotifications(clientId);
        foreach (var notification in list)
        {
            this.Notifications.Add(notification.ToModel());
        }
    }

    [RelayCommand]
    private async void ConfirmDeload(Models.Notification notification)
    {
        if (notification == null)
        {
            return;
        }

        await this.activeWorkoutService.ConfirmDeload(notification.ToDto());
        this.Notifications.Remove(notification);
    }

    private void FocusNextSet(ActiveSetViewModel completedSet)
    {
        foreach (var exercise in this.ExerciseRows)
        {
            foreach (var set in exercise.Sets)
        {
                if (!set.IsCompleted)
                {
                    set.IsFocused = true;
                    return;
                }
            }
        }
    }

    private void UpdateCurrentSetDisplay()
    {
        foreach (var exercise in this.ExerciseRows)
        {
            foreach (var set in exercise.Sets)
            {
                if (!set.IsCompleted)
                {
                    this.currentPendingSet = set;
                    this.CurrentExerciseName = exercise.ExerciseName;
                    this.CurrentTargetReps = set.TargetReps;
                    this.CurrentSetNumber = set.SetIndex;
                    this.CurrentSetRepsInput = set.ActualRepsValue;
                    this.CurrentSetWeightInput = set.ActualWeightValue;
                    return;
                }
            }
        }

        this.currentPendingSet = null;
        this.CurrentExerciseName = "Workout complete";
        this.CurrentTargetReps = null;
        this.CurrentSetNumber = 0;
        this.CurrentSetRepsInput = double.NaN;
        this.CurrentSetWeightInput = double.NaN;
    }

    [RelayCommand]
    private void CompleteCurrentSet()
    {
        if (!this.IsWorkoutStarted || this.currentPendingSet is null)
        {
            return;
        }

        this.currentPendingSet.ActualRepsValue = this.CurrentSetRepsInput;
        this.currentPendingSet.ActualWeightValue = this.CurrentSetWeightInput;
    }
}
