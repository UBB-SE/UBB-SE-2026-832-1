namespace ClassLibrary.DTOs;

public sealed class MealDataTransferObject
{
    public int MealId { get; set; }

    public string Name { get; set; } = string.Empty;

    public List<string> Ingredients { get; set; } = new();

    public string Instructions { get; set; } = string.Empty;
}
