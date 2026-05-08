namespace ClassLibrary.Models;

public sealed class ShoppingListItem
{
    public int ShoppingListItemId { get; set; }

    public string IngredientName { get; set; } = string.Empty;

    public double QuantityGrams { get; set; }

    public bool IsChecked { get; set; }
}
