namespace ClassLibrary.DTOs;

public sealed class AddIngredientByNameRequestDataTransferObject
{
    public int UserId { get; set; }

    public string IngredientName { get; set; } = string.Empty;
}
