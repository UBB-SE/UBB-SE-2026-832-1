namespace ClassLibrary.DTOs;

public sealed class LoggedSetDataTransferObject
{
    public int LoggedSetId { get; set; }

    public string ExerciseName { get; set; } = string.Empty;

    public int SetIndex { get; set; }

    public int TargetReps { get; set; }

    public int ActualReps { get; set; }

    public float TargetWeight { get; set; }

    public float ActualWeight { get; set; }

    public int SetNumber { get; set; }
}