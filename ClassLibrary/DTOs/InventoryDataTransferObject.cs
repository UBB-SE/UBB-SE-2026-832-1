namespace ClassLibrary.DTOs;

public sealed class InventoryDataTransferObject
{
    public int InventoryId { get; set; }

    public int IngredientId { get; set; }

    public string IngredientName { get; set; } = string.Empty;

    public int QuantityGrams { get; set; }
}
