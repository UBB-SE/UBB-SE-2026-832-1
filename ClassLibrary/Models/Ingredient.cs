using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models;

public class Ingredient
{
    [Key]
    public int FoodId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public double CaloriesPer100g { get; set; }

    [Required]
    public double ProteinPer100g { get; set; }

    [Required]
    public double CarbohydratesPer100g { get; set; }

    [Required]
    public double FatPer100g { get; set; }
}
