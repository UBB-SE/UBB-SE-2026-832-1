namespace ClassLibrary.Models;

public class NutritionPlan
{
    public int NutritionPlanId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public List<Meal> Meals { get; set; } = new();
}
