namespace ClassLibrary.Models;

public sealed class FoodItemIngredient
{
    public int FoodItemIngredientId { get; set; }

    public FoodItem FoodItem { get; set; } = default!;

    public Ingredient Ingredient { get; set; } = default!;
}
