using ClassLibrary.DTOs.Analytics;

namespace WebUI.Models.ClientDashboard;

public sealed class WorkoutSessionDetailViewModel
{
    public int TotalCaloriesBurned { get; init; }

    public IReadOnlyList<ExerciseCalorieInfo> ExerciseCalories { get; init; } = [];

    public IReadOnlyList<ExerciseSetGroupViewModel> ExerciseSetGroups { get; init; } = [];

    public static WorkoutSessionDetailViewModel FromDetail(WorkoutSessionDetail detail)
    {
        var grouped = detail.Sets
            .GroupBy(set => set.ExerciseName)
            .ToList();

        var caloriesByExercise = detail.ExerciseCalories
            .Where(calorie => !string.IsNullOrWhiteSpace(calorie.ExerciseName))
            .GroupBy(calorie => calorie.ExerciseName)
            .ToDictionary(
                group => group.Key,
                group => Math.Max(0, group.Sum(item => item.CaloriesBurned)),
                StringComparer.OrdinalIgnoreCase);

        foreach (var exerciseName in grouped
                     .Select(group => group.Key)
                     .Where(name => !string.IsNullOrWhiteSpace(name)))
        {
            if (!caloriesByExercise.ContainsKey(exerciseName))
            {
                caloriesByExercise[exerciseName] = 0;
            }
        }

        var exerciseCalories = caloriesByExercise
            .OrderBy(pair => pair.Key, StringComparer.OrdinalIgnoreCase)
            .Select(pair => new ExerciseCalorieInfo
            {
                ExerciseName = pair.Key,
                CaloriesBurned = pair.Value,
            })
            .ToList();

        var setGroups = grouped
            .Select(group =>
            {
                var exerciseGroup = new ExerciseSetGroupViewModel { ExerciseName = group.Key };
                var setIndex = 1;
                foreach (var set in group)
                {
                    exerciseGroup.Sets.Add(new SetDetailRowViewModel
                    {
                        SetNumber = setIndex++,
                        RepsDisplay = set.ActualReps?.ToString() ?? "—",
                        WeightDisplay = set.ActualWeight?.ToString("F1") ?? "—",
                    });
                }

                return exerciseGroup;
            })
            .ToList();

        return new WorkoutSessionDetailViewModel
        {
            TotalCaloriesBurned = detail.TotalCaloriesBurned,
            ExerciseCalories = exerciseCalories,
            ExerciseSetGroups = setGroups,
        };
    }
}

public sealed class ExerciseSetGroupViewModel
{
    public string ExerciseName { get; init; } = string.Empty;

    public List<SetDetailRowViewModel> Sets { get; } = [];
}

public sealed class SetDetailRowViewModel
{
    public int SetNumber { get; init; }

    public string RepsDisplay { get; init; } = string.Empty;

    public string WeightDisplay { get; init; } = string.Empty;
}
