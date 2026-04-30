namespace ClassLibrary.Models;

public sealed class Inventory
{
    public int InventoryId { get; set; }

    public int UserId { get; set; }

    public User User { get; set; } = default!;

    public int IngredientId { get; set; }

    public Ingredient Ingredient { get; set; } = default!;

    public int QuantityGrams { get; set; }

    public string IngredientName { get; set; } = string.Empty;
}