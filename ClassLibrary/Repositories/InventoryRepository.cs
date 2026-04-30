using ClassLibrary.Data;
using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary.IRepositories;

namespace ClassLibrary.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly AppDbContext _context;

        public InventoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Inventory?> GetByIdAsync(int id)
        {
            return await _context.Inventories.FindAsync(id);
        }

        public async Task<IEnumerable<Inventory>> GetAllByUserIdAsync(int userId)
        {
            
            return await _context.Inventories
                .Include(i => i.Ingredient)
                .Where(i => i.UserId == userId)
                .ToListAsync();
        }

        public async Task AddAsync(Inventory entity)
        {
            
            var existing = await _context.Inventories
                .FirstOrDefaultAsync(i => i.UserId == entity.UserId && i.IngredientId == entity.IngredientId);

            if (existing != null)
            {
                existing.QuantityGrams += entity.QuantityGrams;
                _context.Inventories.Update(existing);
            }
            else
            {
                await _context.Inventories.AddAsync(entity);
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Inventory entity)
        {
            _context.Inventories.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var item = await _context.Inventories.FindAsync(id);
            if (item != null)
            {
                _context.Inventories.Remove(item);
                await _context.SaveChangesAsync();
            }
        }
    }
}