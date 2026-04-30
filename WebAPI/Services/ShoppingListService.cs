using ClassLibrary.DTOs;
using ClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary.IRepositories;
using ClassLibrary.Repositories;

namespace WebApi.Services
{
    public class ShoppingListService : IShoppingListService
    {
        private readonly IShoppingListRepository _shoppingRepository;
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IInventoryRepository _inventoryRepository;

        public ShoppingListService(
            IShoppingListRepository shoppingRepository,
            IIngredientRepository ingredientRepository,
            IInventoryRepository inventoryRepository)
        {
            _shoppingRepository = shoppingRepository;
            _ingredientRepository = ingredientRepository;
            _inventoryRepository = inventoryRepository;
        }
        public async Task GenerateShoppingListFromMealPlanAsync(int userId)
        {
            // Use the repository method we refactored earlier
            var itemsNeeded = await _shoppingRepository.GetIngredientsNeededFromMealPlanAsync(userId);
            foreach (var item in itemsNeeded)
            {
                await _shoppingRepository.AddAsync(item);
            }
        }

        public async Task DeleteAsync(int id)
        {
            await _shoppingRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<ShoppingItem>> GetAllByUserIdAsync(int userId)
        {
            return await _shoppingRepository.GetAllByUserIdAsync(userId);
        }
        public async Task<IEnumerable<ShoppingItemDto>> GetShoppingItemsAsync(int userId)
        {
            var items = await _shoppingRepository.GetAllByUserIdAsync(userId);
            return items.Select(i => new ShoppingItemDto
            {
                Id = i.Id,
                IngredientName = i.IngredientName,
                QuantityGrams = i.QuantityGrams,
                IsChecked = i.IsChecked
            });
        }

        public async Task<ShoppingItemDto?> AddItemAsync(int userId, AddShoppingItemRequest request)
        {
            
            int ingredientId = await _ingredientRepository.GetOrCreateIngredientIdByNameAsync(request.ItemName);

            var item = new ShoppingItem
            {
                UserId = userId,
                IngredientId = ingredientId,
                QuantityGrams = request.Quantity
            };

            await _shoppingRepository.AddAsync(item);

            return new ShoppingItemDto
            {
                Id = item.Id,
                IngredientName = request.ItemName,
                QuantityGrams = item.QuantityGrams
            };
        }

        public async Task<bool> MoveToPantryAsync(int itemId)
        {
            var item = await _shoppingRepository.GetByIdAsync(itemId);
            if (item == null) return false;

            await _inventoryRepository.AddAsync(new Inventory
            {
                UserId = item.UserId,
                IngredientId = item.IngredientId,
                QuantityGrams = item.QuantityGrams > 0 ? (int)item.QuantityGrams : 100
            });

            await _shoppingRepository.DeleteAsync(item.Id);
            return true;
        }

        public async Task<bool> RemoveItemAsync(int itemId) 
        {
            try
            {
                
                await _shoppingRepository.DeleteAsync(itemId);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
