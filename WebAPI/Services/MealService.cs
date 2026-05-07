using ClassLibrary.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.IServices;

namespace WebApi.Services
{
    public class MealService : IMealService
    {
        private readonly IMealRepository _mealRepository;

        // Rule: Constructor Injection
        public MealService(IMealRepository mealRepository)
        {
            _mealRepository = mealRepository;
        }

        public async Task<IEnumerable<FoodItemDto>> GetFilteredMealsAsync(MealFilterDto filter)
        {
            // Fetch from repository
            var meals = await _mealRepository.GetFilteredMealsAsync(filter);

            // Rule: Map Domain Model -> DTO
            return meals.Select(m => new FoodItemDto
            {
                Id = m.Id,
                Name = m.Name,
                Calories = m.Calories,
                Protein = m.Protein,
                Carbs = m.Carbs,
                Fat = m.Fat,
                IsVegan = m.IsVegan,
                // Add other DTO mappings here
            });
        }

        public async Task<FoodItemDto?> GetByIdAsync(int id)
        {
            var m = await _mealRepository.GetByIdAsync(id);
            if (m == null) return null;

            return new FoodItemDto { Id = m.Id, Name = m.Name /* ... */ };
        }

        public async Task ToggleFavoriteAsync(int userId, int mealId, bool isFavorite)
        {
            // Pass logic to repository
            await _mealRepository.SetFavoriteAsync(userId, mealId, isFavorite);
        }

        public async Task<string> GetFormattedIngredientsAsync(int mealId)
        {
            var lines = await _mealRepository.GetIngredientLinesForMealAsync(mealId);
            return lines.Any() ? string.Join("\n", lines) : "No ingredients found.";
        }
    }
}
