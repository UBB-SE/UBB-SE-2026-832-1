namespace ClassLibrary.Models;

public sealed class ShoppingListItem
{
    public int Id { get; set; }

    public string IngredientName { get; set; } = string.Empty;

    public double QuantityGrams { get; set; }

    public bool IsChecked { get; set; }
}
