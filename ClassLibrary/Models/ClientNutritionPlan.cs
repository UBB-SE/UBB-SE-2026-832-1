namespace ClassLibrary.Models;

public class ClientNutritionPlan
{
    public int ClientId { get; set; }

    public int NutritionPlanId { get; set; }

    public NutritionPlan NutritionPlan { get; set; } = null!;
}
