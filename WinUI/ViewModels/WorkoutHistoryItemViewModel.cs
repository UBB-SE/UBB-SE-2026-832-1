using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using ClassLibrary.DTOs.Analytics;
using ClassLibrary.Proxies.Interfaces;

namespace WinUI.ViewModels;

public sealed partial class WorkoutHistoryItemViewModel : ObservableObject
{
    [ObservableProperty]
    private bool isExpanded;

    [ObservableProperty]
    private bool isLoadingDetail;

    private int workoutLogId;

    public int WorkoutLogId => workoutLogId;

    public string Title { get; set; } = string.Empty;

    public string DateLine { get; set; } = string.Empty;

    public string DurationLine { get; set; } = string.Empty;

    public string IntensityTag { get; set; } = string.Empty;

    public int TotalCaloriesBurned { get; set; }

    public ObservableCollection<ExerciseCalorieInfo> ExerciseCalories { get; } = new();

    public ObservableCollection<ExerciseSetGroupViewModel> ExerciseSetGroups { get; } = new();

    public WorkoutHistoryItemViewModel()
    {
    }

    public static WorkoutHistoryItemViewModel FromWorkoutHistoryRow(WorkoutHistoryRow row)
    {
        var vm = new WorkoutHistoryItemViewModel
        {
            workoutLogId = row.Id,
            Title = row.WorkoutName,
            DateLine = row.LogDate.ToString("MMM dd, yyyy"),
            DurationLine = FormatDuration(row.DurationSeconds),
            IntensityTag = row.IntensityTag,
            TotalCaloriesBurned = row.TotalCaloriesBurned,
            IsExpanded = false,
            IsLoadingDetail = false
        };

        return vm;
    }

    public static WorkoutHistoryItemViewModel FromWorkoutSessionDetail(WorkoutSessionDetail detail)
    {
        var vm = new WorkoutHistoryItemViewModel
        {
            workoutLogId = detail.WorkoutLogId,
            Title = detail.WorkoutName,
            DateLine = detail.LogDate.ToString("MMM dd, yyyy"),
            DurationLine = FormatDuration(detail.DurationSeconds),
            IntensityTag = detail.IntensityTag,
            TotalCaloriesBurned = detail.TotalCaloriesBurned,
            IsExpanded = false,
            IsLoadingDetail = false
        };

        foreach (var calorie in detail.ExerciseCalories)
        {
            vm.ExerciseCalories.Add(calorie);
        }

        // Group sets by exercise
        var grouped = detail.Sets.GroupBy(s => s.ExerciseName).ToList();
        foreach (var group in grouped)
        {
            var exerciseGroup = new ExerciseSetGroupViewModel { ExerciseName = group.Key };
            int setIndex = 1;
            foreach (var set in group)
            {
                var setVm = new SetDetailRowViewModel
                {
                    SetNumber = setIndex++,
                    RepsDisplay = set.ActualReps?.ToString() ?? "â€”",
                    WeightDisplay = set.ActualWeight?.ToString("F1") ?? "â€”"
                };
                exerciseGroup.Sets.Add(setVm);
            }
            vm.ExerciseSetGroups.Add(exerciseGroup);
        }

        return vm;
    }

    public void ApplyDetail(WorkoutSessionDetail detail)
    {
        TotalCaloriesBurned = detail.TotalCaloriesBurned;
        OnPropertyChanged(nameof(TotalCaloriesBurned));

        ExerciseSetGroups.Clear();
        var grouped = detail.Sets.GroupBy(set => set.ExerciseName).ToList();
        foreach (var group in grouped)
        {
            var exerciseGroup = new ExerciseSetGroupViewModel { ExerciseName = group.Key };
            int setIndex = 1;
            foreach (var set in group)
            {
                var setVm = new SetDetailRowViewModel
                {
                    SetNumber = setIndex++,
                    RepsDisplay = set.ActualReps?.ToString() ?? "â€”",
                    WeightDisplay = set.ActualWeight?.ToString("F1") ?? "â€”"
                };
                exerciseGroup.Sets.Add(setVm);
            }
            ExerciseSetGroups.Add(exerciseGroup);
        }

        ExerciseCalories.Clear();

        var caloriesByExercise = detail.ExerciseCalories
            .Where(calorie => !string.IsNullOrWhiteSpace(calorie.ExerciseName))
            .GroupBy(calorie => calorie.ExerciseName)
            .ToDictionary(
                group => group.Key,
                group => Math.Max(0, group.Sum(item => item.CaloriesBurned)),
                StringComparer.OrdinalIgnoreCase);

        foreach (var group in grouped)
        {
            if (string.IsNullOrWhiteSpace(group.Key))
            {
                continue;
            }

            if (!caloriesByExercise.ContainsKey(group.Key))
            {
                caloriesByExercise[group.Key] = 0;
            }
        }

        foreach (var pair in caloriesByExercise.OrderBy(pair => pair.Key, StringComparer.OrdinalIgnoreCase))
        {
            ExerciseCalories.Add(new ExerciseCalorieInfo
            {
                ExerciseName = pair.Key,
                CaloriesBurned = pair.Value,
            });
        }
    }

    private static string FormatDuration(int seconds)
    {
        var span = TimeSpan.FromSeconds(seconds);
        if (span.Hours > 0)
        {
            return $"{span.Hours}h {span.Minutes}m";
        }
        return $"{span.Minutes}m";
    }
}

