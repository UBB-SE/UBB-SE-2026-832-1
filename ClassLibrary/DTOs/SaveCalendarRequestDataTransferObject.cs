namespace ClassLibrary.DTOs;

public sealed class SaveCalendarRequestDataTransferObject
{
    public string IcsContent { get; set; } = string.Empty;

    public string? WorkoutName { get; set; }
}
