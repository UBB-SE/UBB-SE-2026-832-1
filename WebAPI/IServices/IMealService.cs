using ClassLibrary.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.IServices
{
    public interface IMealService
    {
        Task<IEnumerable<FoodItemDto>> GetFilteredMealsAsync(MealFilterDto filter);
        Task<FoodItemDto?> GetByIdAsync(int id);
        Task ToggleFavoriteAsync(int userId, int mealId, bool isFavorite);
        Task<string> GetFormattedIngredientsAsync(int mealId);
    }
}
