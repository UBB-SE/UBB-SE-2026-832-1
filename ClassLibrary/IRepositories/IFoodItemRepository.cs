using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Filters;
using ClassLibrary.Models;

namespace ClassLibrary.IRepositories;

public interface IFoodItemRepository
{
    Task<FoodItem?> GetByIdAsync(int id);

    Task<IReadOnlyList<FoodItem>> GetAllAsync();

    Task AddAsync(FoodItem entity);

    Task UpdateAsync(FoodItem entity);

    Task DeleteAsync(int id);

    Task<IReadOnlyList<FoodItem>> GetByFilterAsync(FoodItemFilter filter);

    Task ToggleFavoriteAsync(int userId, int foodItemId);

    Task<IReadOnlyList<FoodItem>> GetFavoritesByUserIdAsync(int userId);
}
