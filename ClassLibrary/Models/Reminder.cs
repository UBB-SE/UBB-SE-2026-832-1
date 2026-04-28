using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassLibrary.Models;

public class Reminder
{
    public int Id { get; set; }

    [Required]
    public virtual User User { get; set; } = null!;

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    public bool HasSound { get; set; }

    [Required]
    public TimeSpan Time { get; set; }

    public string ReminderDate { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Frequency { get; set; } = "Once";

    [NotMapped]
    public string FullDateTimeDisplay => $"{ReminderDate ?? "No date"} at {Time}";
}
