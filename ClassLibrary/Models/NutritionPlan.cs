using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models;

public class NutritionPlan
{
    [Key]
    public int PlanId { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    public List<Meal> Meals { get; set; } = new();
}
