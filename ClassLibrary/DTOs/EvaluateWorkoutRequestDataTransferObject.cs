using ClassLibrary.DTOs;

namespace ClassLibrary.DTOs;

public sealed class EvaluateWorkoutRequestDataTransferObject
{
    public int WorkoutLogId { get; set; }

    public int ClientId { get; set; }

    public List<LoggedExerciseDataTransferObject> Exercises { get; set; } = new();
}
