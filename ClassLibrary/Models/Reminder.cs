using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassLibrary.Models;

public sealed class Reminder
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public bool HasSound { get; set; }

    [Required]
    public TimeSpan Time { get; set; }

    public string ReminderDate { get; set; } = string.Empty;

    public string Frequency { get; set; } = "Once";

    [NotMapped]
    public string FullDateTimeDisplay => $"{ReminderDate ?? "No date"} at {Time}";
}
