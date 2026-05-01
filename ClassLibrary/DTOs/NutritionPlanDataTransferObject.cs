namespace ClassLibrary.DTOs;

public sealed class NutritionPlanDataTransferObject
{
    public int NutritionPlanId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public List<MealDataTransferObject> Meals { get; set; } = new();
}
