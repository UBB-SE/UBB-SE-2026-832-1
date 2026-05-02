using ClassLibrary.DTOs;

namespace WinUI.Services;

public interface IInventoryServiceProxy
{
    Task<IReadOnlyList<InventoryDataTransferObject>> GetUserInventoryAsync(int userId);

    Task<IReadOnlyList<IngredientDataTransferObject>> GetAllIngredientsAsync();

    Task AddToPantryAsync(AddToPantryRequestDataTransferObject request);

    Task AddIngredientByNameToPantryAsync(AddIngredientByNameRequestDataTransferObject request);

    Task<bool> ConsumeMealAsync(ConsumeMealRequestDataTransferObject request);

    Task RemoveItemAsync(int inventoryId);
}
