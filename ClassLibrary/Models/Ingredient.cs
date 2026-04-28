using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models;

public sealed class Ingredient
{
    [Key]
    public int FoodId { get; set; }

    public string Name { get; set; } = string.Empty;

    public double CaloriesPer100g { get; set; }

    public double ProteinPer100g { get; set; }

    public double CarbsPer100g { get; set; }

    public double FatPer100g { get; set; }
}
