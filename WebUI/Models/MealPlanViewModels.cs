using ClassLibrary.DTOs;

namespace WebUI.Models;

public sealed class MealItemViewModel
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
    public bool IsVegan { get; set; }
    public bool IsKeto { get; set; }
    public bool IsGlutenFree { get; set; }
    public bool IsLactoseFree { get; set; }

    public static MealItemViewModel FromFoodItemDto(FoodItemDto dto, int index)
    {
        string mealType = MealTypes.TryGetValue(index, out var type) ? type : "MEAL";
        return new MealItemViewModel
        {
            Id = dto.FoodItemId,
            Name = dto.Name,
            MealType = mealType,
            Description = dto.Description,
            Calories = dto.Calories,
            Protein = dto.Protein,
            Carbohydrates = dto.Carbohydrates,
            Fat = dto.Fat,
            IsVegan = dto.IsVegan,
            IsKeto = dto.IsKeto,
            IsGlutenFree = dto.IsGlutenFree,
            IsLactoseFree = dto.IsLactoseFree,
        };
    }
}

public sealed class MealPlanPageViewModel
{
    public bool HasMeals { get; set; }
    public string StatusMessage { get; set; } = string.Empty;
    public string GoalDescription { get; set; } = string.Empty;
    public int TotalCalories { get; set; }
    public int TotalProtein { get; set; }
    public int TotalCarbohydrates { get; set; }
    public int TotalFat { get; set; }
    public int CurrentMealPlanId { get; set; }
    public List<MealItemViewModel> Meals { get; set; } = new();
    public string? ErrorMessage { get; set; }
}

public sealed class DailyLogPageViewModel
{
    public DailyLogTotalsDto TodayTotals { get; set; } = new();
    public DailyLogTotalsDto WeekTotals { get; set; } = new();
    public UserDataDto? NutritionTargets { get; set; }
    public double TodayBurnedCalories { get; set; }
    public double WeekBurnedCalories { get; set; }
    public IReadOnlyList<FoodItemDto> FoodItems { get; set; } = [];
    public string? SearchTerm { get; set; }
    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }
}
