using System.Net.Http.Json;
using ClassLibrary.DTOs;
using ClassLibrary.Models;

namespace WinUI.Services;

public sealed class ShoppingListService : IShoppingListService
{
    private const string API_BASE_ADDRESS = "https://localhost:7197/api";
    private const string SHOPPING_LIST_ROUTE = "ShoppingList";
    private readonly HttpClient httpClient;

    public ShoppingListService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<IReadOnlyList<ShoppingListItem>> GetShoppingItemsAsync(int userId)
    {
        var shoppingItemDataTransferObjects = await this.httpClient.GetFromJsonAsync<List<ShoppingItemDto>>(
            $"{API_BASE_ADDRESS}/{SHOPPING_LIST_ROUTE}/user/{userId}");

        if (shoppingItemDataTransferObjects is null)
        {
            return Array.Empty<ShoppingListItem>();
        }

        return shoppingItemDataTransferObjects.Select(MapShoppingListItem).ToList();
    }

    public async Task<ShoppingListItem?> AddItemAsync(string itemName, int userId, double quantityGrams)
    {
        var request = new AddShoppingItemRequest
        {
            ItemName = itemName,
            Quantity = quantityGrams,
        };

        HttpResponseMessage response = await this.httpClient.PostAsJsonAsync(
            $"{API_BASE_ADDRESS}/{SHOPPING_LIST_ROUTE}/user/{userId}",
            request);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        IReadOnlyList<ShoppingListItem> latestItems = await this.GetShoppingItemsAsync(userId);
        return latestItems
            .Where(item => item.IngredientName.Equals(itemName, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(item => item.ShoppingListItemId)
            .FirstOrDefault();
    }

    public async Task<bool> RemoveItemAsync(ShoppingListItem item)
    {
        HttpResponseMessage response = await this.httpClient.DeleteAsync(
            $"{API_BASE_ADDRESS}/{SHOPPING_LIST_ROUTE}/{item.ShoppingListItemId}");
        return response.IsSuccessStatusCode;
    }

    public Task<bool> MoveToPantryAsync(ShoppingListItem item)
    {
        // Placeholder until move-to-pantry endpoint is exposed by WebAPI controller.
        return Task.FromResult(false);
    }

    public async Task<int> GenerateListAsync(int userId)
    {
        IReadOnlyList<ShoppingListItem> previousItems = await this.GetShoppingItemsAsync(userId);

        HttpResponseMessage response = await this.httpClient.PostAsync(
            $"{API_BASE_ADDRESS}/{SHOPPING_LIST_ROUTE}/generate/{userId}",
            null);

        if (!response.IsSuccessStatusCode)
        {
            return -1;
        }

        IReadOnlyList<ShoppingListItem> currentItems = await this.GetShoppingItemsAsync(userId);
        return Math.Max(0, currentItems.Count - previousItems.Count);
    }

    public Task<IReadOnlyList<KeyValuePair<int, string>>> SearchIngredientsAsync(string query)
    {
        // Placeholder until ingredient-search endpoint is exposed by WebAPI controller.
        return Task.FromResult<IReadOnlyList<KeyValuePair<int, string>>>(Array.Empty<KeyValuePair<int, string>>());
    }

    private static ShoppingListItem MapShoppingListItem(ShoppingItemDto shoppingItemDataTransferObject)
    {
        return new ShoppingListItem
        {
            ShoppingListItemId = shoppingItemDataTransferObject.Id,
            IngredientName = shoppingItemDataTransferObject.IngredientName,
            QuantityGrams = shoppingItemDataTransferObject.QuantityGrams,
            IsChecked = shoppingItemDataTransferObject.IsChecked,
        };
    }
}
