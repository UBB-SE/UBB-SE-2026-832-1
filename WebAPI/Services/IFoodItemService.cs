using ClassLibrary.DTOs;
using ClassLibrary.Models;

namespace WebAPI.Services;

public interface IFoodItemService
{
    Task<IReadOnlyList<FoodItemDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<FoodItemDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<FoodItemDto>> GetFilteredAsync(FoodItemFilter filter, CancellationToken cancellationToken = default);
}
