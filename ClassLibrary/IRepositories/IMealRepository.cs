using ClassLibrary.DTOs;
using ClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.IRepositories
{
    public interface IMealRepository
    {
        Task<IEnumerable<Meal>> GetFilteredMealsAsync(MealFilterDto filter, int userId);
        Task<List<IngredientDataTransferObject>> GetIngredientsForMealAsync(int mealId);
        Task<Meal?> GetByIdAsync(int id);
        Task SetFavoriteAsync(int userId, int mealId, bool isFavorite);
        Task AddAsync(Meal entity);
        Task UpdateAsync(Meal entity);
        Task DeleteAsync(int id);
    }
}
