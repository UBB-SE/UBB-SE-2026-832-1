using System.ComponentModel.DataAnnotations;

namespace WebUI.Models.Workout;

public class CreateWorkoutFormModel
{
    [Required]
    [MaxLength(200)]
    public string WorkoutName { get; set; } = string.Empty;

    public List<CreateWorkoutExerciseRow> Exercises { get; set; } = new();

    public List<string> AvailableExercises { get; set; } = new();
}

public class CreateWorkoutExerciseRow
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Range(1, 20)]
    public int TargetSets { get; set; } = 3;

    [Range(1, 999)]
    public int TargetReps { get; set; } = 10;

    [Range(0, 1000)]
    public double TargetWeight { get; set; }
}
