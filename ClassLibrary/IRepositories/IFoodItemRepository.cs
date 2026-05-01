using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClassLibrary.Filters;
using ClassLibrary.Models;

namespace ClassLibrary.IRepositories;

public interface IFoodItemRepository
{
    Task<FoodItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<FoodItem>> GetAllAsync(CancellationToken cancellationToken = default);

    Task AddAsync(FoodItem entity, CancellationToken cancellationToken = default);

    Task UpdateAsync(FoodItem entity, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<FoodItem>> GetByFilterAsync(FoodItemFilter filter, CancellationToken cancellationToken = default);

    Task ToggleFavoriteAsync(int userId, int foodItemId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<FoodItem>> GetFavoritesByUserIdAsync(int userId, CancellationToken cancellationToken = default);
}
