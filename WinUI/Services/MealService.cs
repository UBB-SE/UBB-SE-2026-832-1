using System.Net.Http.Json;
using ClassLibrary.DTOs;
using ClassLibrary.Filters;
using ClassLibrary.Models;

namespace WinUI.Services;

public sealed class MealService : IMealService
{
    private const string API_BASE_ADRESS = "https://localhost:7197/api";
    private const string FOOD_ITEMS_ROUTE = "fooditems";
    private readonly HttpClient httpClient;

    public MealService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<IReadOnlyList<FoodItem>> SearchMealsAsync(FoodItemFilter filter)
    {
        string query =
            $"searchTerm={Uri.EscapeDataString(filter.SearchTerm ?? string.Empty)}" +
            $"&isVegan={filter.IsVegan}" +
            $"&isKeto={filter.IsKeto}" +
            $"&isGlutenFree={filter.IsGlutenFree}" +
            $"&isLactoseFree={filter.IsLactoseFree}" +
            $"&isNutFree={filter.IsNutFree}" +
            $"&isFavoriteOnly={filter.IsFavoriteOnly}";

        var foodItemDataTransferObjects =
            await this.httpClient.GetFromJsonAsync<List<FoodItemDto>>(
                $"{API_BASE_ADRESS}/{FOOD_ITEMS_ROUTE}/filter?{query}");

        if (foodItemDataTransferObjects is null)
        {
            return Array.Empty<FoodItem>();
        }

        return foodItemDataTransferObjects.Select(MapFoodItem).ToList();
    }

    public Task ToggleFavoriteAsync(int userId, int mealId)
    {
        // Placeholder until favorite endpoint contract is finalized in WebAPI.
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<string>> GetMealIngredientsAsync(int mealId)
    {
        // Placeholder until ingredients endpoint contract is finalized in WebAPI.
        return Task.FromResult<IReadOnlyList<string>>(Array.Empty<string>());
    }

    private static FoodItem MapFoodItem(FoodItemDto foodItemDataTransferObject)
    {
        return new FoodItem
        {
            FoodItemId = foodItemDataTransferObject.FoodItemId,
            Name = foodItemDataTransferObject.Name,
            Calories = foodItemDataTransferObject.Calories,
            Carbohydrates = foodItemDataTransferObject.Carbohydrates,
            Fat = foodItemDataTransferObject.Fat,
            Protein = foodItemDataTransferObject.Protein,
            IsVegan = foodItemDataTransferObject.IsVegan,
            IsKeto = foodItemDataTransferObject.IsKeto,
            IsGlutenFree = foodItemDataTransferObject.IsGlutenFree,
            IsLactoseFree = foodItemDataTransferObject.IsLactoseFree,
            IsNutFree = foodItemDataTransferObject.IsNutFree,
            Description = foodItemDataTransferObject.Description,
            ImageUrl = foodItemDataTransferObject.ImageUrl,
        };
    }
}
