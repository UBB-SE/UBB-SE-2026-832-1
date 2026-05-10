using System;

namespace ClassLibrary.DTOs;

public sealed class ReminderDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool HasSound { get; set; }
    public TimeSpan Time { get; set; }
    public string ReminderDate { get; set; } = string.Empty;
    public string Frequency { get; set; } = "Once";

    public string FullDateTimeDisplay => $"{ReminderDate} at {Time:hh\\:mm}";
}