using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClassLibrary.DTOs;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using WebAPI.Services.Interfaces;

namespace WebAPI.Services;

public sealed class InventoryService : IInventoryService
{
    private readonly IInventoryRepository inventoryRepository;
    private readonly IMealPlanRepository mealPlanRepository;
    private readonly IIngredientRepository ingredientRepository;

    private const int DEFAULT_INGREDIENTS_QUANTITY = 100;

    public InventoryService(
        IIngredientRepository ingredientRepository,
        IInventoryRepository inventoryRepository,
        IMealPlanRepository mealPlanRepository)
    {
        this.inventoryRepository = inventoryRepository;
        this.mealPlanRepository = mealPlanRepository;
        this.ingredientRepository = ingredientRepository;
    }

    public async Task<bool> ConsumeMealAsync(ConsumeMealRequestDataTransferObject request, CancellationToken cancellationToken = default)
    {
        var requiredIngredientIds = await this.mealPlanRepository.GetIngredientIdsForMealPlanAsync(request.MealPlanId, cancellationToken);
        var requiredQuantityByIngredientId = requiredIngredientIds
            .GroupBy(ingredientId => ingredientId)
            .ToDictionary(
                group => group.Key,
                group => group.Count() * DEFAULT_INGREDIENTS_QUANTITY);

        var inventoryByIngredientId = (await this.inventoryRepository.GetAllByUserIdAsync(request.UserId, cancellationToken))
            .GroupBy(inventoryItem => inventoryItem.Ingredient.IngredientId)
            .ToDictionary(
                group => group.Key,
                group => group.OrderBy(inventoryItem => inventoryItem.InventoryId).ToList());

        foreach (var requiredIngredient in requiredQuantityByIngredientId)
        {
            if (!inventoryByIngredientId.TryGetValue(requiredIngredient.Key, out var stocks) ||
                stocks.Sum(stock => stock.QuantityGrams) < requiredIngredient.Value)
            {
                return false;
            }
        }

        foreach (var requiredIngredient in requiredQuantityByIngredientId)
        {
            int remainingQuantityToConsume = requiredIngredient.Value;

            foreach (var stock in inventoryByIngredientId[requiredIngredient.Key])
            {
                if (remainingQuantityToConsume <= 0)
                {
                    break;
                }

                int quantityToConsume = stock.QuantityGrams >= remainingQuantityToConsume
                    ? remainingQuantityToConsume
                    : stock.QuantityGrams;

                stock.QuantityGrams -= quantityToConsume;
                remainingQuantityToConsume -= quantityToConsume;

                if (stock.QuantityGrams <= 0)
                {
                    await this.inventoryRepository.DeleteAsync(stock.InventoryId, cancellationToken);
                }
                else
                {
                    await this.inventoryRepository.UpdateAsync(stock, cancellationToken);
                }
            }
        }

        return true;
    }

    public async Task AddToPantryAsync(AddToPantryRequestDataTransferObject request, CancellationToken cancellationToken = default)
    {
        var newItem = new Inventory
        {
            User = new User { UserId = request.UserId },
            Ingredient = new Ingredient { IngredientId = request.IngredientId },
            QuantityGrams = request.QuantityGrams,
        };

        await this.inventoryRepository.AddAsync(newItem, cancellationToken);
    }

    public async Task AddIngredientByNameToPantryAsync(AddIngredientByNameRequestDataTransferObject request, CancellationToken cancellationToken = default)
    {
        int ingredientId = await this.ingredientRepository.GetOrCreateIngredientIdByNameAsync(request.IngredientName);
        await this.AddToPantryAsync(new AddToPantryRequestDataTransferObject
        {
            UserId = request.UserId,
            IngredientId = ingredientId,
            QuantityGrams = DEFAULT_INGREDIENTS_QUANTITY,
        }, cancellationToken);
    }

    public async Task<IReadOnlyList<InventoryDataTransferObject>> GetUserInventoryAsync(int userId, CancellationToken cancellationToken = default)
    {
        var inventoryItems = await this.inventoryRepository.GetAllByUserIdAsync(userId, cancellationToken);
        return inventoryItems.Select(MapToInventoryDto).ToList();
    }

    public async Task RemoveItemAsync(int inventoryId, CancellationToken cancellationToken = default)
    {
        await this.inventoryRepository.DeleteAsync(inventoryId, cancellationToken);
    }

    public async Task<IReadOnlyList<IngredientDataTransferObject>> GetAllIngredientsAsync(CancellationToken cancellationToken = default)
    {
        var ingredients = await this.ingredientRepository.GetAllAsync();
        return ingredients.Select(MapToIngredientDto).ToList();
    }

    private static InventoryDataTransferObject MapToInventoryDto(Inventory inventory)
    {
        return new InventoryDataTransferObject
        {
            InventoryId = inventory.InventoryId,
            IngredientId = inventory.Ingredient.IngredientId,
            IngredientName = inventory.Ingredient.Name,
            QuantityGrams = inventory.QuantityGrams,
        };
    }

    private static IngredientDataTransferObject MapToIngredientDto(Ingredient ingredient)
    {
        return new IngredientDataTransferObject
        {
            IngredientId = ingredient.IngredientId,
            Name = ingredient.Name,
            CaloriesPer100g = ingredient.CaloriesPer100g,
            ProteinPer100g = ingredient.ProteinPer100g,
            CarbohydratesPer100g = ingredient.CarbohydratesPer100g,
            FatPer100g = ingredient.FatPer100g,
        };
    }
}
