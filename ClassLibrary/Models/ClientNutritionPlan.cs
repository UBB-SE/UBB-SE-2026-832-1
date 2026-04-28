using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models;

public class ClientNutritionPlan
{
    [Required]
    public Client Client { get; set; } = null!;

    [Required]
    public NutritionPlan NutritionPlan { get; set; } = null!;
}
