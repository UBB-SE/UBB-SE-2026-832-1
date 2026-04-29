using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models;
public class Ingredient
{
    [Key]
    public int IngredientId { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    public double CaloriesPer100g { get; set; }

    public double ProteinPer100g { get; set; }

    public double CarbohydratesPer100g { get; set; }

    public double FatPer100g { get; set; }
}