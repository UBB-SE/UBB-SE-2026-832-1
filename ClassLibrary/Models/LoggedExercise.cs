using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models;

public sealed class LoggedExercise
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string ExerciseName { get; set; } = string.Empty;

    [Required]
    public int WorkoutLogId { get; set; }

    public WorkoutLog WorkoutLog { get; set; } = null!;

    public int ParentTemplateExerciseId { get; set; }

    public List<LoggedSet> Sets { get; set; } = new();

    [Required]
    public MuscleGroup TargetMuscles { get; set; }

    public float MetabolicEquivalent { get; set; }

    public int ExerciseCaloriesBurned { get; set; }

    public double PerformanceRatio { get; set; }

    public bool IsSystemAdjusted { get; set; }

    [MaxLength(500)]
    public string AdjustmentNote { get; set; } = string.Empty;
}
