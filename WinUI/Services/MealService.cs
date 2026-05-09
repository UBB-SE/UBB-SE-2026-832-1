using ClassLibrary.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary.IRepositories;
using WinUI.IServices;

namespace WinUI.Services
{
    public class MealService : IMealService
    {
        private readonly IMealRepository _mealRepository;

        
        public MealService(IMealRepository mealRepository)
        {
            _mealRepository = mealRepository;
        }

        public async Task<IEnumerable<FoodItemDto>> GetFilteredMealsAsync(MealFilterDto filter, int userId)
        {
         
            var meals = await _mealRepository.GetFilteredMealsAsync(filter, userId);

            return meals.Select(m => new FoodItemDto
            {
                FoodItemId = m.MealId,
                Name = m.Name,
                IsVegan = m.IsVegan
            });
        }

        
        public async Task<FoodItemDto?> GetByIdAsync(int id)
        {
            var m = await _mealRepository.GetByIdAsync(id);
            if (m == null) return null;

            return new FoodItemDto { FoodItemId = m.MealId, Name = m.Name };
        }

        public async Task ToggleFavoriteAsync(FavoriteRequestDto request)
        {
            
            await _mealRepository.SetFavoriteAsync(request.UserId, request.MealId, request.IsFavorite);
        }

        public async Task<string> GetFormattedIngredientsAsync(int mealId)
        {
            var lines = await _mealRepository.GetIngredientsForMealAsync(mealId);
            return lines.Any() ? string.Join("\n", lines) : "No ingredients found.";
        }
    }
}
