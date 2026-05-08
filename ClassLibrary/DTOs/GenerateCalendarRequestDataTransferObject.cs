namespace ClassLibrary.DTOs;

public sealed class GenerateCalendarRequestDataTransferObject
{
    public int WorkoutTemplateId { get; set; }

    public int DurationWeeks { get; set; }

    public List<DayOfWeek> SelectedDays { get; set; } = [];

    public DateTime? StartDate { get; set; }
}
