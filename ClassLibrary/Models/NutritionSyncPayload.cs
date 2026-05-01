using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models;

public class NutritionSyncPayload
{
    public int NutritionSyncPayloadId { get; set; }

    [Required]
    public int TotalCalories { get; set; }

    [Required]
    [MaxLength(50)]
    public string WorkoutDifficulty { get; set; } = string.Empty;

    [Required]
    [Range(0, 100)]
    public float UserBmi { get; set; }
}