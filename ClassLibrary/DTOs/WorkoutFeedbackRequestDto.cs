namespace ClassLibrary.DTOs;

public sealed class WorkoutFeedbackRequestDto
{
    public int LogId { get; set; }
    public int Rating { get; set; }
    public string TrainerNotes { get; set; } = string.Empty;
}
