using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models;

public sealed class Meal
{
    [Key]
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int Calories { get; set; }

    public int Carbs { get; set; }

    public int Fat { get; set; }

    public int Protein { get; set; }

    public bool IsVegan { get; set; }

    public bool IsKeto { get; set; }

    public bool IsGlutenFree { get; set; }

    public bool IsLactoseFree { get; set; }

    public bool IsNutFree { get; set; }

    public bool IsFavorite { get; set; }

    public string Description { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;
}
