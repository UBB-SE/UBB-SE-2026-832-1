using ClassLibrary.Models;

namespace ClassLibrary.DTOs;

public sealed class TemplateExerciseDto
{
    public int Id { get; set; }
    public string ExerciseName { get; set; } = string.Empty;
    public int TargetSets { get; set; }
    public int TargetReps { get; set; }
    public MuscleGroup MuscleGroup { get; set; }
    public double TargetWeight { get; set; }
}
