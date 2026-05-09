using System.Collections.Generic;
using ClassLibrary.DTOs;
using Microsoft.UI.Xaml;

namespace WinUI.ViewModels;

public class MealViewModel
{
    private static readonly Dictionary<int, string> MealTypes = new()
    {
        { 0, "BREAKFAST" },
        { 1, "LUNCH" },
        { 2, "DINNER" },
    };

    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string MealType { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int Calories { get; set; }

    public int Protein { get; set; }

    public int Carbohydrates { get; set; }

    public int Fat { get; set; }

    public Visibility IsVegan { get; set; }

    public Visibility IsKeto { get; set; }

    public Visibility IsGlutenFree { get; set; }

    public Visibility IsLactoseFree { get; set; }

    public Visibility FavoriteVisibility { get; set; }

    public static MealViewModel FromFoodItemDto(FoodItemDto dto, int index)
    {
        string mealType = MealTypes.TryGetValue(index, out var type) ? type : "MEAL";
        return new MealViewModel
        {
            Id = dto.FoodItemId,
            Name = dto.Name,
            MealType = mealType,
            Description = dto.Description,
            Calories = dto.Calories,
            Protein = dto.Protein,
            Carbohydrates = dto.Carbohydrates,
            Fat = dto.Fat,
            IsVegan = dto.IsVegan ? Visibility.Visible : Visibility.Collapsed,
            IsKeto = dto.IsKeto ? Visibility.Visible : Visibility.Collapsed,
            IsGlutenFree = dto.IsGlutenFree ? Visibility.Visible : Visibility.Collapsed,
            IsLactoseFree = dto.IsLactoseFree ? Visibility.Visible : Visibility.Collapsed,
            FavoriteVisibility = Visibility.Collapsed,
        };
    }
}
