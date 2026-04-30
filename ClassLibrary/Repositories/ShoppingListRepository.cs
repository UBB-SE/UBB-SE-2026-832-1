using ClassLibrary.Data;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Repositories
{
    public class ShoppingListRepository : IShoppingListRepository
    {
        private readonly AppDbContext _context;

        public ShoppingListRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddAsync(ShoppingItem item)
        {
            await _context.ShoppingItems.AddAsync(item);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ShoppingItem>> GetAllAsync()
        {
            return await _context.ShoppingItems
                .Include(s => s.Ingredient)
                .ToListAsync();
        }

        public async Task<ShoppingItem?> GetByIdAsync(int id)
        {
            return await _context.ShoppingItems
                .Include(s => s.Ingredient)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<ShoppingItem?> GetByUserAndIngredientAsync(int userId, int ingredientId)
        {
            return await _context.ShoppingItems
                .FirstOrDefaultAsync(s => s.UserId == userId && s.IngredientId == ingredientId);
        }

        public async Task<IEnumerable<ShoppingItem>> GetAllByUserIdAsync(int userId)
        {
            return await _context.ShoppingItems
                .Where(s => s.UserId == userId)
                .ToListAsync();
        }

        public async Task UpdateAsync(ShoppingItem item)
        {
            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var item = await _context.ShoppingItems.FindAsync(id);
            if (item != null)
            {
                _context.ShoppingItems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<ShoppingItem>> GetIngredientsNeededFromMealPlanAsync(int userId)
        {
            
            var latestMealPlan = await _context.MealPlans
                .Where(mp => mp.UserId == userId && mp.CreatedAt.Date == DateTime.Now.Date)
                .OrderByDescending(mp => mp.CreatedAt)
                .FirstOrDefaultAsync();

            if (latestMealPlan == null) return new List<ShoppingItem>();

            
            var mealPlanItems = await _context.MealPlanFoodItems
                .Where(mpfi => mpfi.MealPlanId == latestMealPlan.MealPlanId)
                .Include(mpfi => mpfi.FoodItem)
                .ToListAsync();

            var finalShoppingList = new List<ShoppingItem>();

            foreach (var mpItem in mealPlanItems)
            {
                if (mpItem.FoodItem == null) continue;

                
                double inventoryQty = await _context.Inventories
                    .Where(inv => inv.UserId == userId && inv.IngredientId == mpItem.FoodItemId)
                    .Select(inv => (double?)inv.QuantityGrams)
                    .FirstOrDefaultAsync() ?? 0;

                
                double existingShoppingQty = await _context.ShoppingItems
                    .Where(si => si.UserId == userId && si.IngredientId == mpItem.FoodItemId)
                    .Select(si => (double?)si.QuantityGrams)
                    .FirstOrDefaultAsync() ?? 0;

                
                double requiredQty = 100.0;
                double totalNeeded = requiredQty - (inventoryQty + existingShoppingQty);

                if (totalNeeded > 0)
                {
                    finalShoppingList.Add(new ShoppingItem
                    {
                        UserId = userId,
                        IngredientId = mpItem.FoodItemId,
                        IngredientName = mpItem.FoodItem.Name,
                        QuantityGrams = totalNeeded,
                        IsChecked = false
                    });
                }
            }

            return finalShoppingList;
        }
    }
}