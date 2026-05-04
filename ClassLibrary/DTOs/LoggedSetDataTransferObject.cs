namespace ClassLibrary.DTOs;

public sealed class LoggedExerciseDataTransferObject
{
    public int LoggedExerciseId { get; set; }

    public string ExerciseName { get; set; } = string.Empty;

    public string TargetMuscles { get; set; } = string.Empty;

    public List<LoggedSetDataTransferObject> Sets { get; set; } = new();

    public float MetabolicEquivalent { get; set; }

    public int ExerciseCaloriesBurned { get; set; }

    public double PerformanceRatio { get; set; }

    public bool IsSystemAdjusted { get; set; }

    public string AdjustmentNote { get; set; } = string.Empty;

    public int ParentTemplateExerciseId { get; set; }
}
