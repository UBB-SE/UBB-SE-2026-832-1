namespace ClassLibrary.DTOs;

public sealed class AddToPantryRequestDataTransferObject
{
    public int UserId { get; set; }

    public int IngredientId { get; set; }

    public int QuantityGrams { get; set; }
}
