namespace ClassLibrary.Models;

public sealed class FoodItemIngredient
{
    public int FoodItemIngredientId { get; set; }

    public int FoodItemId { get; set; }

    public FoodItem FoodItem { get; set; } = default!;

    public int IngredientId { get; set; }

    public Ingredient Ingredient { get; set; } = default!;
}
