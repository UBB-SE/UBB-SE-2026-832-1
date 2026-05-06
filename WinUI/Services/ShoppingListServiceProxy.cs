using System.Net.Http.Json;
using ClassLibrary.DTOs;
using WinUI.Services.Interfaces;

namespace WinUI.Services;

public sealed class ShoppingListServiceProxy : IShoppingListServiceProxy
{
    private readonly HttpClient httpClient;

    public ShoppingListServiceProxy(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<IReadOnlyList<ShoppingItemDto>> GetShoppingItemsAsync(int userId)
    {
        var items = await this.httpClient.GetFromJsonAsync<List<ShoppingItemDto>>(
            $"{ApiBaseUrl.BASE_URL}/api/ShoppingList/user/{userId}");
        return items ?? [];
    }

    public async Task<bool> AddItemAsync(string itemName, int userId, double quantityGrams)
    {
        var request = new AddShoppingItemRequest
        {
            ItemName = itemName,
            Quantity = quantityGrams,
        };

        var response = await this.httpClient.PostAsJsonAsync(
            $"{ApiBaseUrl.BASE_URL}/api/ShoppingList/user/{userId}",
            request);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> RemoveItemAsync(int itemId)
    {
        var response = await this.httpClient.DeleteAsync(
            $"{ApiBaseUrl.BASE_URL}/api/ShoppingList/{itemId}");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> MoveToPantryAsync(int itemId)
    {
        var response = await this.httpClient.PostAsync(
            $"{ApiBaseUrl.BASE_URL}/api/ShoppingList/{itemId}/move-to-pantry",
            null);
        return response.IsSuccessStatusCode;
    }

    public async Task<int> GenerateListAsync(int userId)
    {
        var previousItems = await this.GetShoppingItemsAsync(userId);

        var response = await this.httpClient.PostAsync(
            $"{ApiBaseUrl.BASE_URL}/api/ShoppingList/generate/{userId}",
            null);

        if (!response.IsSuccessStatusCode)
        {
            return -1;
        }

        var currentItems = await this.GetShoppingItemsAsync(userId);
        return Math.Max(0, currentItems.Count - previousItems.Count);
    }

    public async Task<IReadOnlyList<IngredientDataTransferObject>> GetAllIngredientsAsync()
    {
        var ingredients = await this.httpClient.GetFromJsonAsync<List<IngredientDataTransferObject>>(
            $"{ApiBaseUrl.BASE_URL}/api/inventory/ingredients");
        return ingredients ?? [];
    }
}
