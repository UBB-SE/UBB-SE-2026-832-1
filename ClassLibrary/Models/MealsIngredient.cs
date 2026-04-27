namespace ClassLibrary.Models;

public sealed class MealsIngredient
{
    public int MealId { get; set; }

    public int FoodId { get; set; }

    public double Quantity { get; set; }
}
