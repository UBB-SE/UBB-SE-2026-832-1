using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models;

public class WorkoutTemplate
{
    public int WorkoutTemplateId { get; set; }

    public Client Client { get; set; } = null!;

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public WorkoutType Type { get; set; }

    public ICollection<TemplateExercise> Exercises { get; set; } = [];
}
