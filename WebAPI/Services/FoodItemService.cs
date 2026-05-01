using ClassLibrary.DTOs;
using ClassLibrary.Filters;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;

namespace WebAPI.Services;

public sealed class FoodItemService : IFoodItemService
{
    private readonly IFoodItemRepository foodItemRepository;

    public FoodItemService(IFoodItemRepository foodItemRepository)
    {
        this.foodItemRepository = foodItemRepository;
    }

    public async Task<IReadOnlyList<FoodItemDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var foodItems = await this.foodItemRepository.GetAllAsync(cancellationToken);
        return MapToDtos(foodItems);
    }

    public async Task<FoodItemDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var foodItem = await this.foodItemRepository.GetByIdAsync(id, cancellationToken);
        return foodItem is null ? null : MapToDto(foodItem);
    }

    public async Task<IReadOnlyList<FoodItemDto>> GetFilteredAsync(FoodItemFilter filter, CancellationToken cancellationToken = default)
    {
        var foodItems = await this.foodItemRepository.GetByFilterAsync(filter, cancellationToken);
        return MapToDtos(foodItems);
    }

    private static FoodItemDto MapToDto(FoodItem foodItem)
    {
        return new FoodItemDto
        {
            FoodItemId = foodItem.FoodItemId,
            Name = foodItem.Name,
            Calories = foodItem.Calories,
            Carbohydrates = foodItem.Carbohydrates,
            Fat = foodItem.Fat,
            Protein = foodItem.Protein,
            IsVegan = foodItem.IsVegan,
            IsKeto = foodItem.IsKeto,
            IsGlutenFree = foodItem.IsGlutenFree,
            IsLactoseFree = foodItem.IsLactoseFree,
            IsNutFree = foodItem.IsNutFree,
            Description = foodItem.Description,
            ImageUrl = foodItem.ImageUrl,
        };
    }

    private static IReadOnlyList<FoodItemDto> MapToDtos(IReadOnlyList<FoodItem> foodItems)
    {
        return foodItems.Select(MapToDto).ToList();
    }
}
