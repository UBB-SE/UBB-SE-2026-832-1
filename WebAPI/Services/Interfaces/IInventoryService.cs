using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClassLibrary.DTOs;

namespace WebAPI.Services.Interfaces;

public interface IInventoryService
{
    Task<bool> ConsumeMealAsync(ConsumeMealRequestDataTransferObject request, CancellationToken cancellationToken = default);

    Task AddToPantryAsync(AddToPantryRequestDataTransferObject request, CancellationToken cancellationToken = default);

    Task AddIngredientByNameToPantryAsync(AddIngredientByNameRequestDataTransferObject request, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<InventoryDataTransferObject>> GetUserInventoryAsync(int userId, CancellationToken cancellationToken = default);

    Task RemoveItemAsync(int inventoryId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<IngredientDataTransferObject>> GetAllIngredientsAsync(CancellationToken cancellationToken = default);
}
