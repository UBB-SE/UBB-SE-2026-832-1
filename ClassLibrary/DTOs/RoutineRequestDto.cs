using System.Collections.Generic;

namespace ClassLibrary.DTOs;

public sealed class RoutineRequestDto
{
    public int? EditingTemplateId { get; set; }
    public int ClientId { get; set; }
    public string RoutineName { get; set; } = string.Empty;
    public List<TemplateExerciseDto> Exercises { get; set; } = new();
}
