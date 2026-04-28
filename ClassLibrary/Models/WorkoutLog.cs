using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models;

public sealed class WorkoutLog
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int ClientId { get; set; }

    [Required]
    [MaxLength(200)]
    public string WorkoutName { get; set; } = string.Empty;

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public TimeSpan Duration { get; set; }

    public int SourceTemplateId { get; set; }

    [Required]
    public WorkoutType Type { get; set; }

    public List<LoggedExercise> Exercises { get; set; } = new();

    public int TotalCaloriesBurned { get; set; }

    public float AverageMetabolicEquivalent { get; set; }

    [MaxLength(100)]
    public string IntensityTag { get; set; } = string.Empty;

    public double Rating { get; set; }

    [MaxLength(1000)]
    public string TrainerNotes { get; set; } = string.Empty;
}
