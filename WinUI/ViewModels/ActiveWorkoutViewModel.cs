using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using ClassLibrary.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using WinUI.Services;

namespace WinUI.ViewModels;

public sealed partial class ActiveWorkoutViewModel : ObservableObject
{
    private const int DEFAULT_REST_TIME_SECONDS = 90;
    private const int REST_TIMER_INTERVAL_MS = 1000;
    private const int HOUR_IN_SECONDS = 3600;

    private readonly IActiveWorkoutService activeWorkoutService;
    private readonly WorkoutUiState workoutUiState;

    private int clientId;
    private WorkoutLog activeLog;
    private ActiveSetViewModel? currentPendingSet;
    private System.Timers.Timer? restTimer;
    private DispatcherTimer? elapsedTimer;
    private TimeSpan elapsedWorkout;

    public event Action? WorkoutFinished;

    public ActiveWorkoutViewModel(IActiveWorkoutService activeWorkoutService, WorkoutUiState workoutUiState)
    {
        this.activeWorkoutService = activeWorkoutService;
        this.workoutUiState = workoutUiState;
        this.activeLog = new WorkoutLog { Date = DateTime.Now, Client = new Client() };
    }

    [ObservableProperty]
    private string workoutElapsedDisplay = "00:00";

    [ObservableProperty]
    private string workoutSessionTitle = string.Empty;

    [ObservableProperty]
    private string currentExerciseName = string.Empty;

    [ObservableProperty]
    private int? currentTargetReps;

    [ObservableProperty]
    private int currentSetNumber;

    [ObservableProperty]
    private double currentSetRepsInput = double.NaN;

    [ObservableProperty]
    private double currentSetWeightInput = double.NaN;

    public string CurrentSetRepsInputText
    {
        get => double.IsNaN(this.CurrentSetRepsInput) ? string.Empty : ((int)this.CurrentSetRepsInput).ToString();
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                this.CurrentSetRepsInput = double.NaN;
                return;
            }
            if (int.TryParse(value, out int parsed))
            {
                this.CurrentSetRepsInput = parsed;
            }
        }
    }
    private static double GetExerciseIntensityMultiplier(
    MuscleGroup muscleGroup)
    {
        return muscleGroup switch
        {
            MuscleGroup.CHEST => 0.11,
            MuscleGroup.BACK => 0.12,
            MuscleGroup.LEGS => 0.15,
            MuscleGroup.SHOULDERS => 0.10,
            MuscleGroup.ARMS => 0.08,
            MuscleGroup.CORE => 0.07,
            MuscleGroup.CARDIO => 0.18,
            _ => 0.10
        };
    }
    public string CurrentSetWeightInputText
    {
        get => double.IsNaN(this.CurrentSetWeightInput) ? string.Empty : this.CurrentSetWeightInput.ToString("0.##");
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                this.CurrentSetWeightInput = double.NaN;
                return;
            }
            if (double.TryParse(value, out double parsed))
            {
                this.CurrentSetWeightInput = parsed;
            }
        }
    }

    [ObservableProperty]
    private int restTimeRemaining;

    [ObservableProperty]
    private bool isResting;

    [ObservableProperty]
    private ObservableCollection<WorkoutTemplate> availableWorkouts = new();

    [ObservableProperty]
    private ObservableCollection<WorkoutTemplate> customWorkouts = new();

    [ObservableProperty]
    private bool hasCustomWorkouts;

    [ObservableProperty]
    private WorkoutTemplate? selectedTemplate;

    [ObservableProperty]
    private bool isLoadingWorkouts;

    [ObservableProperty]
    private string selectedGoal = string.Empty;

    [ObservableProperty]
    private ObservableCollection<ActiveExerciseViewModel> exerciseRows = new();

    [ObservableProperty]
    private ActiveExerciseViewModel? currentExercise;

    [ObservableProperty]
    private bool isWorkoutStarted;

    [ObservableProperty]
    private bool isFinishing;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private ObservableCollection<Notification> notifications = new();

    public WorkoutLog? LastCompletedLog { get; private set; }

    partial void OnCurrentSetRepsInputChanged(double value)
    {
        OnPropertyChanged(nameof(CurrentSetRepsInputText));
    }

    partial void OnCurrentSetWeightInputChanged(double value)
    {
        OnPropertyChanged(nameof(CurrentSetWeightInputText));
    }

    partial void OnIsWorkoutStartedChanged(bool value)
    {
        if (value)
        {
            this.elapsedWorkout = TimeSpan.Zero;
            this.WorkoutElapsedDisplay = "00:00";
            StartWorkoutElapsedTimer();
        }
        else
        {
            StopWorkoutElapsedTimer();
        }
    }

    partial void OnSelectedTemplateChanged(WorkoutTemplate? value)
    {
        if (value is null)
        {
            return;
        }

        this.IsWorkoutStarted = false;

        this.activeLog = new WorkoutLog
        {
            WorkoutName = value.Name,
            SourceTemplateId = value.WorkoutTemplateId,
            Type = value.Type,
            Date = DateTime.Now,
            Client = new Client { ClientId = this.clientId },
        };

        this.ExerciseRows.Clear();
        foreach (var exercise in value.Exercises)
        {
            this.ExerciseRows.Add(new ActiveExerciseViewModel(exercise, SaveSet));
        }

        UpdateCurrentSetDisplay();
        this.WorkoutSessionTitle = value.Name;
        this.IsWorkoutStarted = true;
    }

    public async void LoadCustomWorkouts(int clientId)
    {
        this.clientId = clientId;
        this.activeLog = new WorkoutLog
        {
            Date = DateTime.Now,
            Client = new Client { ClientId = clientId },
        };

        var workouts = await this.activeWorkoutService.GetCustomAndTrainerAssignedWorkoutsForClient(clientId);
        this.CustomWorkouts.Clear();
        foreach (var w in workouts)
        {
            this.CustomWorkouts.Add(w);
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
    private async Task ApplyTargetGoals(int clientId)
    {
        if (string.IsNullOrEmpty(this.SelectedGoal))
        {
            return;
        }

        try
        {
            this.IsLoadingWorkouts = true;
            var allWorkouts = await this.activeWorkoutService.GetAvailableWorkoutsForClient(clientId);
            var selected = allWorkouts.Where(w => w.Name == this.SelectedGoal).ToList();

            if (selected.Count == 0)
            {
                return;
            }

            this.IsWorkoutStarted = false;

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
                Client = new Client { ClientId = clientId },
            };

            this.ExerciseRows.Clear();
            foreach (var template in selected)
            {
                foreach (var exercise in template.Exercises)
                {
                    this.ExerciseRows.Add(new ActiveExerciseViewModel(exercise, SaveSet));
                }
            }

            UpdateCurrentSetDisplay();
            this.WorkoutSessionTitle = this.activeLog.WorkoutName;
            this.IsWorkoutStarted = true;
        }
        catch (Exception ex)
        {
            this.ErrorMessage = $"Error applying goals: {ex.Message}";
        }
        finally
        {
            this.IsLoadingWorkouts = false;
        }
    }

    private void SaveSet(ActiveSetViewModel setViewModel)
    {
        _ = SaveSetAsync(setViewModel);
    }

    private async Task SaveSetAsync(ActiveSetViewModel setViewModel)
    {
        if (setViewModel is null || !this.IsWorkoutStarted)
        {
            return;
        }

        this.ErrorMessage = string.Empty;

        var set = new LoggedSet
        {
            ExerciseName = setViewModel.ExerciseName,
            SetIndex = setViewModel.SetIndex,
            SetNumber = setViewModel.SetIndex,
            ActualReps = setViewModel.ActualReps,
            ActualWeight = setViewModel.ActualWeight,
            TargetReps = setViewModel.TargetReps,
            WorkoutLog = this.activeLog,
        };

        double multiplier =
            GetExerciseIntensityMultiplier(
                this.CurrentExercise?.MuscleGroup
                ?? MuscleGroup.CHEST);

        double setCalories =
            ((set.ActualReps ?? 0)
            * (set.ActualWeight ?? 0)
            * multiplier) / 10.0;

        var loggedExercise =
            this.activeLog.Exercises
                .FirstOrDefault(
                    exercise =>
                        exercise.ExerciseName ==
                        set.ExerciseName);

        if (loggedExercise == null)
        {
            loggedExercise = new LoggedExercise
            {
                ExerciseName = set.ExerciseName,

                TargetMuscles =
                    this.CurrentExercise?.MuscleGroup
                    ?? MuscleGroup.CHEST,

                Sets = new List<LoggedSet>(),

                ExerciseCaloriesBurned = 0,
            };

            this.activeLog.Exercises.Add(loggedExercise);
        }

        loggedExercise.Sets.Add(set);

        loggedExercise.ExerciseCaloriesBurned += (int)setCalories;

        loggedExercise.MetabolicEquivalent =
            (float)multiplier * 50;

        this.activeLog.TotalCaloriesBurned =
            this.activeLog.Exercises.Sum(
                exercise =>
                    exercise.ExerciseCaloriesBurned);

        this.activeLog.AverageMetabolicEquivalent =
            this.activeLog.Exercises.Average(
                exercise =>
                    exercise.MetabolicEquivalent);

        bool isSaved =
            await this.activeWorkoutService.SaveSetAsync(
                this.activeLog,
                set);

        if (!isSaved)
        {
            this.ErrorMessage =
                "Failed to save set. Please try again.";

            return;
        }

        setViewModel.IsCompleted = true;

        FocusNextSet(setViewModel);

        UpdateCurrentSetDisplay();
    }

    [RelayCommand]
    private async Task FinishWorkout(int clientId)
    {
        if (!this.IsWorkoutStarted)
        {
            return;
        }

        try
        {
            this.IsFinishing = true;
            this.ErrorMessage = string.Empty;

            this.activeLog.Client = new Client { ClientId = clientId };
            this.activeLog.Duration = this.elapsedWorkout;

            bool success = await this.activeWorkoutService.FinalizeWorkoutAsync(this.activeLog);

            if (success)
            {
                this.LastCompletedLog = this.activeLog;
                this.workoutUiState.ProgressionHeadsUp = BuildProgressionHeadsUp(this.activeLog);
                SaveWorkoutCalendar(this.activeLog);
                AnalyticsDashboardRefreshBus.Shared.RequestRefresh();
                this.IsWorkoutStarted = false;
                this.ExerciseRows.Clear();
                this.activeLog = new WorkoutLog { Date = DateTime.Now, Client = new Client { ClientId = clientId } };
                this.WorkoutSessionTitle = string.Empty;
                this.CurrentExerciseName = string.Empty;
                this.CurrentTargetReps = null;
                this.CurrentSetNumber = 0;
                this.CurrentSetRepsInput = double.NaN;
                this.CurrentSetWeightInput = double.NaN;

                WorkoutFinished?.Invoke();
            }
            else
            {
                this.ErrorMessage = "Failed to save workout. Please try again.";
            }
        }
        catch (Exception ex)
        {
            this.ErrorMessage = $"Error finishing workout: {ex.Message}";
        }
        finally
        {
            this.IsFinishing = false;
        }
    }

    [RelayCommand]
    private async Task RepeatWorkout(int clientId)
    {
        if (this.LastCompletedLog is null)
        {
            return;
        }

        var template = await this.activeWorkoutService.FindWorkoutTemplateById(
            clientId, this.LastCompletedLog.SourceTemplateId);

        if (template is null)
        {
            return;
        }

        this.SelectedTemplate = template;
    }

    [RelayCommand]
    private async Task LoadNotifications(int clientId)
    {
        this.Notifications.Clear();
        var list = await this.activeWorkoutService.GetNotifications(clientId);
        foreach (var n in list)
        {
            this.Notifications.Add(n);
        }
    }

    [RelayCommand]
    private async Task ConfirmDeload(Notification notification)
    {
        if (notification is null)
        {
            return;
        }

        await this.activeWorkoutService.ConfirmDeload(notification);
        this.Notifications.Remove(notification);
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

    public void StartRestTimer(int seconds = DEFAULT_REST_TIME_SECONDS)
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
        this.restTimer = new System.Timers.Timer(REST_TIMER_INTERVAL_MS);
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

        this.restTimer.Start();
    }

    private void StartWorkoutElapsedTimer()
    {
        StopWorkoutElapsedTimer();
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
                    this.CurrentExercise = exercise;
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
        this.CurrentExercise = null;
        this.CurrentExerciseName = "Workout complete";
        this.CurrentTargetReps = null;
        this.CurrentSetNumber = 0;
        this.CurrentSetRepsInput = double.NaN;
        this.CurrentSetWeightInput = double.NaN;
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

    private static void SaveWorkoutCalendar(WorkoutLog workoutLog)
    {
        try
        {
            string icsContent = BuildIcsContent(workoutLog);
            string folder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "VibeCoders",
                "WorkoutCalendar");
            Directory.CreateDirectory(folder);
            string fileName = $"workout-{workoutLog.Date:yyyyMMdd-HHmmss}.ics";
            string filePath = Path.Combine(folder, fileName);
            File.WriteAllText(filePath, icsContent, Encoding.UTF8);
        }
        catch
        {
        }
    }

    private static string BuildIcsContent(WorkoutLog workoutLog)
    {
        DateTime startUtc = workoutLog.Date.ToUniversalTime();
        DateTime endUtc = workoutLog.Duration > TimeSpan.Zero
            ? startUtc.Add(workoutLog.Duration)
            : startUtc.AddMinutes(30);
        string workoutName = string.IsNullOrWhiteSpace(workoutLog.WorkoutName) ? "Workout" : workoutLog.WorkoutName;
        string description = $"Calories burned: {workoutLog.TotalCaloriesBurned:F0} kcal\\nExercises: {workoutLog.Exercises.Count}";

        var builder = new StringBuilder();
        builder.AppendLine("BEGIN:VCALENDAR");
        builder.AppendLine("VERSION:2.0");
        builder.AppendLine("PRODID:-//VibeCoders//WorkoutLog//EN");
        builder.AppendLine("BEGIN:VEVENT");
        builder.AppendLine($"UID:{Guid.NewGuid()}@vibecoders");
        builder.AppendLine($"DTSTAMP:{DateTime.UtcNow:yyyyMMddTHHmmssZ}");
        builder.AppendLine($"DTSTART:{startUtc:yyyyMMddTHHmmssZ}");
        builder.AppendLine($"DTEND:{endUtc:yyyyMMddTHHmmssZ}");
        builder.AppendLine($"SUMMARY:{workoutName}");
        builder.AppendLine($"DESCRIPTION:{description}");
        builder.AppendLine("END:VEVENT");
        builder.AppendLine("END:VCALENDAR");
        return builder.ToString();
    }
}
