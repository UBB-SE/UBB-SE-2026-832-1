using System;

namespace ClassLibrary.Models;

public class Reminder
{
    public int Id { get; set; }

    public virtual User User { get; set; } = null!;

    public string Name { get; set; } = string.Empty;

    public bool HasSound { get; set; }

    public TimeSpan Time { get; set; }

    public string ReminderDate { get; set; } = string.Empty;

    public string Frequency { get; set; } = "Once";

    public string FullDateTimeDisplay => $"{ReminderDate ?? "No date"} at {Time}";
}
