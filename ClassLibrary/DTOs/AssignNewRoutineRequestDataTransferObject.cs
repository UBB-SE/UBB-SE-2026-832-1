namespace ClassLibrary.DTOs;

public sealed class AssignNewRoutineRequestDataTransferObject
{
    public int? EditingTemplateId { get; set; }

    public int ClientId { get; set; }

    public string RoutineName { get; set; } = string.Empty;

    public List<TemplateExerciseDataTransferObject> Exercises { get; set; } = new();
}
