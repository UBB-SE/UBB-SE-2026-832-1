namespace ClassLibrary.DTOs;

public sealed class SaveWorkoutFeedbackRequestDataTransferObject
{
    public int WorkoutLogId { get; set; }

    public double Rating { get; set; }

    public string TrainerNotes { get; set; } = string.Empty;
}
