using System.Collections.Generic;

namespace ClassLibrary.DTOs;

public sealed class CalendarWorkoutCatalogResponseDataTransferObject
{
    public List<WorkoutTemplateDataTransferObject> Workouts { get; set; } = new();
}
