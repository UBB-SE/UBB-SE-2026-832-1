using ClassLibrary.DTOs;
using ClassLibrary.Filters;

namespace WebAPI.IServices;

public interface IFoodItemService
{
    Task<IReadOnlyList<FoodItemDto>> GetAllAsync();

    Task<FoodItemDto?> GetByIdAsync(int id);

    Task<IReadOnlyList<FoodItemDto>> GetFilteredAsync(FoodItemFilter filter);
}
