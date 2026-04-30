namespace ClassLibrary.DTOs;

using System;
using System.Collections.Generic;

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

public sealed class WorkoutFeedbackRequestDto
{
    public int LogId { get; set; }
    public int Rating { get; set; }
    public string TrainerNotes { get; set; } = string.Empty;
}

public sealed class NutritionPlanRequestDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int ClientId { get; set; }
}

public sealed class RoutineRequestDto
{
    public int? EditingTemplateId { get; set; }
    public int ClientId { get; set; }
    public string RoutineName { get; set; } = string.Empty;
    public List<TemplateExerciseDto> Exercises { get; set; } = new();
}

public sealed class TemplateExerciseDto
{
    public int Id { get; set; }
    public string ExerciseName { get; set; } = string.Empty;
    public int TargetSets { get; set; }
    public int TargetReps { get; set; }
    public string MuscleGroup { get; set; } = string.Empty;
    public decimal TargetWeight { get; set; }
}

public sealed class WorkoutTemplateDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public sealed class ClientDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
