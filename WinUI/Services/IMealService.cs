using ClassLibrary.Filters;
using ClassLibrary.Models;

namespace WinUI.Services;

public interface IMealService
{
    Task<IReadOnlyList<FoodItem>> SearchMealsAsync(FoodItemFilter filter);

    Task ToggleFavoriteAsync(int userId, int mealId);

    Task<IReadOnlyList<string>> GetMealIngredientsAsync(int mealId);
}
