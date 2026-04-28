namespace ClassLibrary.Models;

public sealed class MealPlanFoodItem
{
    public int MealPlanId { get; set; }

    public MealPlan MealPlan { get; set; } = default!;

    public int FoodItemId { get; set; }

    public FoodItem FoodItem { get; set; } = default!;
}
