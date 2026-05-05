namespace ClassLibrary.DTOs;

public sealed class WorkoutTemplateDataTransferObject
{
    public int WorkoutTemplateId { get; set; }

    public int ClientId { get; set; }

    public ClientDataTransferObject Client { get; set; } = new();

    public string Name { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public List<TemplateExerciseDataTransferObject> Exercises { get; set; } = [];
}
