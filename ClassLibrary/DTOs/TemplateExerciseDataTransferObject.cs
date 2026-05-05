namespace ClassLibrary.DTOs;

public sealed class TemplateExerciseDataTransferObject
{
    public string Name { get; set; } = string.Empty;

    public string MuscleGroup { get; set; } = string.Empty;

    public int TargetSets { get; set; }

    public int TargetReps { get; set; }

    public double TargetWeight { get; set; }
}
