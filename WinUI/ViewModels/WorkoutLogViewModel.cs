using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary.DTOs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WinUI.Services;

namespace WinUI.ViewModels;

public sealed partial class WorkoutLogViewModel : ObservableObject
{
    private const string ERROR_LOAD_FORMAT = "Failed to load workout logs: {0}";
    private const string ERROR_SAVE = "Failed to save workout changes.";
    private const string ERROR_SAVE_FORMAT = "Failed to save workout changes: {0}";

    private readonly IWorkoutLogService workoutLogService;

    public ObservableCollection<WorkoutLogItemViewModel> Logs { get; } = new();

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private bool showEmptyState;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public WorkoutLogViewModel(IWorkoutLogService workoutLogService)
    {
        this.workoutLogService = workoutLogService ?? throw new ArgumentNullException(nameof(workoutLogService));
    }

    [RelayCommand]
    private async Task LoadLogsAsync(int clientId)
    {
        try
        {
            this.IsLoading = true;
            this.ErrorMessage = string.Empty;
            this.Logs.Clear();
            this.ShowEmptyState = false;

            var logs = await this.workoutLogService.GetWorkoutHistoryAsync(clientId);
            foreach (var workoutLog in logs)
            {
                this.Logs.Add(new WorkoutLogItemViewModel(workoutLog));
            }

            this.ShowEmptyState = this.Logs.Count == 0;
        }
        catch (Exception exception)
        {
            this.Logs.Clear();
            this.ShowEmptyState = true;
            this.ErrorMessage = string.Format(ERROR_LOAD_FORMAT, exception.Message);
        }
        finally
        {
            this.IsLoading = false;
        }
    }

    [RelayCommand]
    private void ToggleEditMode(WorkoutLogItemViewModel item)
    {
        if (item == null)
        {
            return;
        }

        if (item.IsEditMode)
        {
            item.CancelEditMode();
        }
        else
        {
            item.EnterEditMode();
        }
    }

    [RelayCommand]
    private void SaveEditedLog(WorkoutLogItemViewModel item)
    {
        if (item == null || !item.IsEditMode)
        {
            return;
        }

        try
        {
            this.ErrorMessage = string.Empty;
            item.CommitEditMode();
        }
        catch (Exception exception)
        {
            this.ErrorMessage = string.Format(ERROR_SAVE_FORMAT, exception.Message);
        }
    }
}

public sealed partial class WorkoutLogItemViewModel : ObservableObject
{
    private readonly WorkoutLogDataTransferObject workoutLogDto;

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

    public WorkoutLogItemViewModel(WorkoutLogDataTransferObject workoutLogDto)
    {
        this.workoutLogDto = workoutLogDto;
        this.Id = workoutLogDto.WorkoutLogId;
        this.WorkoutName = string.IsNullOrWhiteSpace(workoutLogDto.WorkoutName) ? "Workout" : workoutLogDto.WorkoutName;
        this.Date = workoutLogDto.Date;
        this.TypeDisplay = workoutLogDto.Type?.ToUpperInvariant() switch
        {
            "PREBUILT" => "PRE-BUILT",
            "TRAINER_ASSIGNED" => "TRAINER ASSIGNED",
            _ => "CUSTOM"
        };

        this.DateDisplay = workoutLogDto.Date.ToString("yyyy-MM-dd");
        this.TotalDurationDisplay = FormatDuration(workoutLogDto.Duration);

        this.LoadExercisesFromDto(workoutLogDto);
    }

    public void EnterEditMode() => this.IsEditMode = true;

    public void CancelEditMode()
    {
        this.LoadExercisesFromDto(this.workoutLogDto);
        this.IsEditMode = false;
    }

    public void CommitEditMode()
    {
        this.IsEditMode = false;
    }

    private void LoadExercisesFromDto(WorkoutLogDataTransferObject dto)
    {
        this.Exercises.Clear();
        foreach (var exercise in dto.Exercises)
        {
            this.Exercises.Add(new WorkoutLogExerciseSummary(exercise));
        }
    }

    private static string FormatDuration(TimeSpan duration)
    {
        if (duration.TotalHours >= 1)
        {
            return $"{(int)duration.TotalHours}h {duration.Minutes:D2}m";
        }

        return $"{duration.Minutes}m";
    }
}

public sealed class WorkoutLogExerciseSummary
{
    public string ExerciseName { get; }

    public bool IsSystemAdjusted { get; }

    public string TooltipText { get; }

    public ObservableCollection<WorkoutLogSetEditorViewModel> Sets { get; } = new();

    public int NumberOfSets => this.Sets.Count;

    public string RepsDisplay => this.Sets.Count > 0
        ? string.Join(" / ", this.Sets.Select(setEditor => setEditor.RepsDisplay))
        : "\u2014";

    public string WeightDisplay => this.Sets.Count > 0
        ? string.Join(" / ", this.Sets.Select(setEditor => setEditor.WeightDisplay))
        : "\u2014";

    public WorkoutLogExerciseSummary(LoggedExerciseDataTransferObject exercise)
    {
        this.ExerciseName = exercise.ExerciseName;
        this.IsSystemAdjusted = exercise.IsSystemAdjusted;

        this.TooltipText = !string.IsNullOrWhiteSpace(exercise.AdjustmentNote)
            ? exercise.AdjustmentNote
            : $"Performance: {exercise.PerformanceRatio * 100:F0}% of target reps achieved.";

        int index = 1;
        foreach (var set in exercise.Sets.OrderBy(loggedSet => loggedSet.SetIndex))
        {
            this.Sets.Add(new WorkoutLogSetEditorViewModel
            {
                SetNumber = index++,
                Reps = set.ActualReps,
                Weight = set.ActualWeight,
            });
        }
    }
}

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

    public string RepsDisplay => this.Reps?.ToString(CultureInfo.InvariantCulture) ?? "\u2014";

    public string WeightDisplay => this.Weight.HasValue
        ? $"{this.Weight.Value.ToString("0.##", CultureInfo.InvariantCulture)} kg"
        : "\u2014";

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
