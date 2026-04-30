using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models;

public class WorkoutTemplate
{
    public int WorkoutTemplateId { get; set; }

    [Required]
    public int ClientId { get; set; }

    [Required]
    public Client Client { get; set; } = null!;

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public WorkoutType Type { get; set; }

    public ICollection<TemplateExercise> Exercises { get; set; } = [];
}
