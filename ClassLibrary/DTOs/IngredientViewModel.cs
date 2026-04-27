namespace ClassLibrary.DTOs;

public sealed class IngredientViewModel
{
    public int IngredientId { get; set; }

    public string Name { get; set; } = string.Empty;

    public double Quantity { get; set; }

    public double Calories { get; set; }

    public double Protein { get; set; }

    public double Carbs { get; set; }

    public double Fat { get; set; }
}
