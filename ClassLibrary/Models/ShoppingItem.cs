namespace ClassLibrary.Models;

public sealed class ShoppingItem
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int IngredientId { get; set; }

    public string IngredientName { get; set; } = string.Empty;

    public double QuantityGrams { get; set; }

    public bool IsChecked { get; set; }
}
