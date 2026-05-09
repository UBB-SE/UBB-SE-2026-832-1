using ClassLibrary.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinUI.IServices
{
public interface IMealService
{
        Task<IEnumerable<FoodItemDto>> GetFilteredMealsAsync(MealFilterDto filter, int userId);
        Task<FoodItemDto?> GetByIdAsync(int id);
        Task ToggleFavoriteAsync(FavoriteRequestDto request);
        Task<string> GetFormattedIngredientsAsync(int mealId);
    }
}
