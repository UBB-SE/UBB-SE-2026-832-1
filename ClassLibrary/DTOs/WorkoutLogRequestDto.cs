namespace ClassLibrary.DTOs;

public class WorkoutLogRequestDto
{
    public long UserId { get; set; }
    public List<ExerciseRequestDto> Exercises { get; set; } = new();
}