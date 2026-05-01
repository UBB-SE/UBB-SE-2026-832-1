namespace ClassLibrary.DTOs;

public sealed class FinalizeWorkoutRequestDataTransferObject
{
    public WorkoutLogDataTransferObject WorkoutLog { get; set; } = new();
}
