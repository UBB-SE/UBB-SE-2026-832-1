namespace ClassLibrary.Models;

public sealed class MealPlanFoodItem
{
    public int MealPlanFoodItemId { get; set; }

    public MealPlan MealPlan { get; set; } = default!;

    public FoodItem FoodItem { get; set; } = default!;
}
