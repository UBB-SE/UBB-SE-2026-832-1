public class Reminder
{
    public int ReminderId { get; set; } 

    public int UserId { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool HasSound { get; set; }

    public TimeSpan Time { get; set; }

    public string? ReminderDate { get; set; }

    public string Frequency { get; set; } = "Once";
}