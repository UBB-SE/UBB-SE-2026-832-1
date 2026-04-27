namespace ClassLibrary.Models;

public sealed class Inventory
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int IngredientId { get; set; }

    public int QuantityGrams { get; set; }

    public string IngredientName { get; set; } = string.Empty;
}
