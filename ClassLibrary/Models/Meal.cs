using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models;

public class Meal
{
    public int MealId { get; set; }

    [Required]
    public NutritionPlan NutritionPlan { get; set; } = null!;

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public List<string> Ingredients { get; set; } = new();

    [MaxLength(2000)]
    public string Instructions { get; set; } = string.Empty;
}
