namespace ClassLibrary.Models;

public class ClientNutritionPlan
{
    public Client Client { get; set; } = null!;

    public NutritionPlan NutritionPlan { get; set; } = null!;
}
