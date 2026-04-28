using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models;

/// <summary>
/// A catalogued food item with nutritional data and dietary flags.
/// Distinct from <see cref="Meal"/>, which represents a recipe inside a <see cref="NutritionPlan"/>.
/// </summary>
public sealed class FoodItem
{
    public int FoodItemId { get; set; }

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
