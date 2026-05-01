using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models;

public class TemplateExercise
{
    public int TemplateExerciseId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public WorkoutTemplate WorkoutTemplate { get; set; } = null!;

    public MuscleGroup MuscleGroup { get; set; }

    public int TargetSets { get; set; }

    public int TargetReps { get; set; }

    public double TargetWeight { get; set; }
}
