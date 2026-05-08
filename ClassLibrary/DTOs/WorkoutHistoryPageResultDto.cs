namespace ClassLibrary.DTOs;

public class WorkoutHistoryPageResultDto
{
    public int TotalCount { get; set; }
    public List<WorkoutHistoryItemDto> Items { get; set; } = new();
}

public class WorkoutHistoryItemDto
{
    public long WorkoutId { get; set; }
    public DateTime Date { get; set; }
    public string ActivityName { get; set; } = string.Empty;
    public double Duration { get; set; }
}
