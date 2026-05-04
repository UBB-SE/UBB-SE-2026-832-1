using ClassLibrary.DTOs;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.IServices;

namespace WebAPI.Services
{
    public class ShoppingListService : IShoppingListService
    {
        private readonly IShoppingListRepository shoppingRepository;
        private readonly IIngredientRepository ingredientRepository;
        private readonly IInventoryRepository inventoryRepository;
        private readonly IMealPlanRepository mealPlanRepository;

        public ShoppingListService(
            IShoppingListRepository shoppingRepository,
            IIngredientRepository ingredientRepository,
            IInventoryRepository inventoryRepository,
            IMealPlanRepository mealPlanRepository)
        {
            this.shoppingRepository = shoppingRepository;
            this.ingredientRepository = ingredientRepository;
            this.inventoryRepository = inventoryRepository;
            this.mealPlanRepository = mealPlanRepository;
        }

        public async Task GenerateShoppingListFromMealPlanAsync(int userId)
        {
            var mealPlans = await mealPlanRepository.GetByUserIdAsync(userId);
            var latestMealPlan = mealPlans
                .OrderByDescending(mp => mp.MealPlanId)
                .FirstOrDefault();

            if (latestMealPlan == null) return;

            var ingredientIds = await mealPlanRepository.GetIngredientIdsForMealPlanAsync(latestMealPlan.MealPlanId);
            var inventoryItems = await inventoryRepository.GetAllByUserIdAsync(userId);
            var existingShoppingItems = await shoppingRepository.GetAllByUserIdAsync(userId);

            var inventoryByIngredientId = inventoryItems
                .GroupBy(inv => inv.Ingredient.IngredientId)
                .ToDictionary(g => g.Key, g => g.Sum(inv => (double)inv.QuantityGrams));

            var shoppingByIngredientId = existingShoppingItems
                .GroupBy(si => si.Ingredient.IngredientId)
                .ToDictionary(g => g.Key, g => g.Sum(si => si.QuantityGrams));

            const double requiredQty = 100.0;

            foreach (var ingredientId in ingredientIds.Distinct())
            {
                var existingInventoryQty = inventoryByIngredientId.GetValueOrDefault(ingredientId, 0);
                var existingShoppingQty = shoppingByIngredientId.GetValueOrDefault(ingredientId, 0);
                var totalNeeded = requiredQty - (existingInventoryQty + existingShoppingQty);

                if (totalNeeded > 0)
                {
                    var item = new ShoppingItem
                    {
                        User = new User { UserId = userId },
                        Ingredient = new Ingredient { IngredientId = ingredientId },
                        QuantityGrams = totalNeeded,
                        IsChecked = false
                    };
                    await shoppingRepository.AddAsync(item);
                }
            }
        }

        public async Task DeleteAsync(int id)
        {
            await shoppingRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<ShoppingItemDto>> GetShoppingItemsAsync(int userId)
        {
            var items = await shoppingRepository.GetAllByUserIdAsync(userId);
            return items.Select(i => new ShoppingItemDto
            {
                Id = i.ShoppingItemId,
                IngredientName = i.Ingredient?.Name ?? string.Empty,
                QuantityGrams = i.QuantityGrams,
                IsChecked = i.IsChecked
            });
        }

        public async Task<ShoppingItemDto?> AddItemAsync(int userId, AddShoppingItemRequest request)
        {
            int ingredientId = await ingredientRepository.GetOrCreateIngredientIdByNameAsync(request.ItemName);

            var item = new ShoppingItem
            {
                User = new User { UserId = userId },
                Ingredient = new Ingredient { IngredientId = ingredientId },
                QuantityGrams = request.Quantity
            };

            await shoppingRepository.AddAsync(item);

            return new ShoppingItemDto
            {
                Id = item.ShoppingItemId,
                IngredientName = request.ItemName,
                QuantityGrams = item.QuantityGrams
            };
        }

        public async Task<bool> MoveToPantryAsync(int itemId)
        {
            var item = await shoppingRepository.GetByIdAsync(itemId);
            if (item == null) return false;

            var userId = item.User?.UserId ?? 0;
            if (userId <= 0) return false;

            await inventoryRepository.AddAsync(new Inventory
            {
                User = new User { UserId = userId },
                Ingredient = new Ingredient { IngredientId = item.Ingredient.IngredientId },
                QuantityGrams = item.QuantityGrams > 0 ? (int)item.QuantityGrams : 100
            });

            await shoppingRepository.DeleteAsync(item.ShoppingItemId);
            return true;
        }

        public async Task<bool> RemoveItemAsync(int itemId)
        {
            try
            {
                await shoppingRepository.DeleteAsync(itemId);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
