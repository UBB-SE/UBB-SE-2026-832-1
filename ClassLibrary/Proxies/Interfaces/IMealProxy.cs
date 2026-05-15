using ClassLibrary.Filters;
using ClassLibrary.Models;

namespace ClassLibrary.Proxies.Interfaces;

public interface IMealProxy
{
    Task<IReadOnlyList<FoodItem>> SearchMealsAsync(FoodItemFilter filter);

    Task ToggleFavoriteAsync(int userId, int mealId);

    Task<IReadOnlyList<string>> GetMealIngredientsAsync(int mealId);
}



