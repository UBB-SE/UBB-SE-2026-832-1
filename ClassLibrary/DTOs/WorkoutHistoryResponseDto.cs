namespace ClassLibrary.DTOs;

using System;

public sealed class WorkoutHistoryResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public int Rating { get; set; }
    public string TrainerNotes { get; set; } = string.Empty;
    public DateTime Date { get; set; }
}
