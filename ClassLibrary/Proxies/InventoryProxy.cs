using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using ClassLibrary.DTOs;
using ClassLibrary.Proxies.Interfaces;

namespace ClassLibrary.Proxies;

public sealed class InventoryProxy : IInventoryProxy
{
    private readonly HttpClient httpClient;
    private readonly IInventoryProxy? serviceProxy;

    public InventoryProxy()
    {
        this.httpClient = new HttpClient();
    }

    public InventoryProxy(IInventoryProxy serviceProxy)
    {
        this.serviceProxy = serviceProxy;
        this.httpClient = new HttpClient();
    }

    public InventoryProxy(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<IReadOnlyList<InventoryDataTransferObject>> GetUserInventoryAsync(int userId)
    {
        if (this.serviceProxy != null)
        {
            return await this.serviceProxy.GetUserInventoryAsync(userId);
        }

        var inventoryItems = await this.httpClient.GetFromJsonAsync<List<InventoryDataTransferObject>>(
            $"{ApiBaseUrl.BASE_URL}/api/inventory/{userId}");
        return inventoryItems ?? [];
    }

    public async Task<IReadOnlyList<IngredientDataTransferObject>> GetAllIngredientsAsync()
    {
        if (this.serviceProxy != null)
        {
            return await this.serviceProxy.GetAllIngredientsAsync();
        }

        var ingredients = await this.httpClient.GetFromJsonAsync<List<IngredientDataTransferObject>>(
            $"{ApiBaseUrl.BASE_URL}/api/inventory/ingredients");
        return ingredients ?? [];
    }

    public async Task AddToPantryAsync(AddToPantryRequestDataTransferObject request)
    {
        if (this.serviceProxy != null)
        {
            await this.serviceProxy.AddToPantryAsync(request);
            return;
        }

        var response = await this.httpClient.PostAsJsonAsync(
            $"{ApiBaseUrl.BASE_URL}/api/inventory/add-to-pantry",
            request);
        response.EnsureSuccessStatusCode();
    }

    public async Task AddIngredientByNameToPantryAsync(AddIngredientByNameRequestDataTransferObject request)
    {
        if (this.serviceProxy != null)
        {
            await this.serviceProxy.AddIngredientByNameToPantryAsync(request);
            return;
        }

        var response = await this.httpClient.PostAsJsonAsync(
            $"{ApiBaseUrl.BASE_URL}/api/inventory/add-ingredient-by-name",
            request);
        response.EnsureSuccessStatusCode();
    }

    public async Task<bool> ConsumeMealAsync(ConsumeMealRequestDataTransferObject request)
    {
        if (this.serviceProxy != null)
        {
            return await this.serviceProxy.ConsumeMealAsync(request);
        }

        var response = await this.httpClient.PostAsJsonAsync(
            $"{ApiBaseUrl.BASE_URL}/api/inventory/consume-meal",
            request);

        if (response.IsSuccessStatusCode)
        {
            return true;
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            return false;
        }

        response.EnsureSuccessStatusCode();
        return true;
    }

    public async Task RemoveItemAsync(int inventoryId)
    {
        if (this.serviceProxy != null)
        {
            await this.serviceProxy.RemoveItemAsync(inventoryId);
            return;
        }

        var response = await this.httpClient.DeleteAsync($"{ApiBaseUrl.BASE_URL}/api/inventory/{inventoryId}");
        response.EnsureSuccessStatusCode();
    }
}


