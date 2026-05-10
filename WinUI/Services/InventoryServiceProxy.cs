using System.Net.Http.Json;
using ClassLibrary.DTOs;

namespace WinUI.Services;

public sealed class InventoryServiceProxy : IInventoryServiceProxy
{
    private readonly HttpClient httpClient;

    public InventoryServiceProxy(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<IReadOnlyList<InventoryDataTransferObject>> GetUserInventoryAsync(int userId)
    {
        var inventory = await this.httpClient.GetFromJsonAsync<List<InventoryDataTransferObject>>($"{ApiBaseUrl.BASE_URL}/api/inventory/{userId}");
        return inventory ?? [];
    }

    public async Task<IReadOnlyList<IngredientDataTransferObject>> GetAllIngredientsAsync()
    {
        var ingredients = await this.httpClient.GetFromJsonAsync<List<IngredientDataTransferObject>>($"{ApiBaseUrl.BASE_URL}/api/inventory/ingredients");
        return ingredients ?? [];
    }

    public async Task AddToPantryAsync(AddToPantryRequestDataTransferObject request)
    {
        var response = await this.httpClient.PostAsJsonAsync($"{ApiBaseUrl.BASE_URL}/api/inventory/add-to-pantry", request);
        response.EnsureSuccessStatusCode();
    }

    public async Task AddIngredientByNameToPantryAsync(AddIngredientByNameRequestDataTransferObject request)
    {
        var response = await this.httpClient.PostAsJsonAsync($"{ApiBaseUrl.BASE_URL}/api/inventory/add-ingredient-by-name", request);
        response.EnsureSuccessStatusCode();
    }

    public async Task<bool> ConsumeMealAsync(ConsumeMealRequestDataTransferObject request)
    {
        var response = await this.httpClient.PostAsJsonAsync($"{ApiBaseUrl.BASE_URL}/api/inventory/consume-meal", request);
        return response.IsSuccessStatusCode;
    }

    public async Task RemoveItemAsync(int inventoryId)
    {
        var response = await this.httpClient.DeleteAsync($"{ApiBaseUrl.BASE_URL}/api/inventory/{inventoryId}");
        response.EnsureSuccessStatusCode();
    }
}
