namespace ClassLibrary.DTOs;

public sealed class WorkoutLogDataTransferObject
{
    public int WorkoutLogId { get; set; }

    public ClientDataTransferObject Client { get; set; } = new();

    public string WorkoutName { get; set; } = string.Empty;

    public DateTime Date { get; set; }

    public TimeSpan Duration { get; set; }

    public int? SourceTemplateId { get; set; }

    public string Type { get; set; } = string.Empty;

    public List<LoggedExerciseDataTransferObject> Exercises { get; set; } = new();

    public int TotalCaloriesBurned { get; set; }

    public float AverageMetabolicEquivalent { get; set; }

    public string IntensityTag { get; set; } = string.Empty;

    public double Rating { get; set; }

    public string TrainerNotes { get; set; } = string.Empty;
}
