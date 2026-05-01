using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models;

public sealed class LoggedExercise
{
    public int LoggedExerciseId { get; set; }

    [Required]
    [MaxLength(200)]
    public string ExerciseName { get; set; } = string.Empty;

    public WorkoutLog WorkoutLog { get; set; } = null!;

    public int ParentTemplateExerciseId { get; set; }

    public List<LoggedSet> Sets { get; set; } = new();

    public MuscleGroup TargetMuscles { get; set; }

    public float MetabolicEquivalent { get; set; }

    public int ExerciseCaloriesBurned { get; set; }

    public double PerformanceRatio { get; set; }

    public bool IsSystemAdjusted { get; set; }

    [MaxLength(500)]
    public string AdjustmentNote { get; set; } = string.Empty;
}
