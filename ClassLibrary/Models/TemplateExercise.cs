using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models;

public class TemplateExercise
{
    public int TemplateExerciseId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public WorkoutTemplate WorkoutTemplate { get; set; } = null!;

    [Required]
    public MuscleGroup MuscleGroup { get; set; }

    [Required]
    public int TargetSets { get; set; }

    [Required]
    public int TargetReps { get; set; }

    [Required]
    public double TargetWeight { get; set; }
}
