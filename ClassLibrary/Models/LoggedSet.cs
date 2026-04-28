using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models;

public sealed class LoggedSet
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int WorkoutLogId { get; set; }

    public WorkoutLog WorkoutLog { get; set; } = null!;

    [Required]
    [MaxLength(200)]
    public string ExerciseName { get; set; } = string.Empty;

    public int SetIndex { get; set; }

    public int? TargetReps { get; set; }

    public int? ActualReps { get; set; }

    public double? TargetWeight { get; set; }

    public double? ActualWeight { get; set; }

    public int SetNumber { get; set; }

    public LoggedExercise? Exercise { get; set; }
}
