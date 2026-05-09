using ClassLibrary.DTOs;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;
using ClassLibrary.Data; 

namespace ClassLibrary.Repositories
{
    public class MealRepository : IMealRepository
    {
        private readonly AppDbContext _context;

       
        public MealRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<IngredientDataTransferObject>> GetIngredientsForMealAsync(int mealId)
        {
            var meal = await _context.Meals.Include(m => m.Ingredients).FirstOrDefaultAsync(m => m.MealId == mealId);

            if (meal == null) return new List<IngredientDataTransferObject>();

           
            return meal.Ingredients.Select(i => new IngredientDataTransferObject
            {
                Name = i, 
                Quantity = 0 
            }).ToList();
        }

        public async Task<IEnumerable<Meal>> GetFilteredMealsAsync(MealFilterDto filter, int userId)
        {
            IQueryable<Meal> query = _context.Meals
                .Include(m => m.NutritionPlan)
                .AsQueryable();

            
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
                query = query.Where(m => m.Name.Contains(filter.SearchTerm));

            if (filter.IsVegan) query = query.Where(m => m.IsVegan);
            if (filter.IsKeto) query = query.Where(m => m.IsKeto);
            if (filter.IsGlutenFree) query = query.Where(m => m.IsGlutenFree);
            if (filter.IsLactoseFree) query = query.Where(m => m.IsLactoseFree);
            if (filter.IsNutFree) query = query.Where(m => m.IsNutFree);

            
            if (filter.IsFavoriteOnly)
            {
                query = query.Where(m => _context.Favorites
                    .Any(f => f.MealId == m.MealId && f.UserId == userId));
            }

            return await query.ToListAsync();
        }

        public async Task<Meal?> GetByIdAsync(int id)
        {
            return await _context.Meals
                .Include(m => m.NutritionPlan)
                .FirstOrDefaultAsync(m => m.MealId == id);
        }

        public async Task SetFavoriteAsync(int userId, int mealId, bool isFavorite)
        {
            var existing = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.MealId == mealId);

            if (isFavorite && existing == null)
            {
                _context.Favorites.Add(new Favorite { UserId = userId, MealId = mealId });
            }
            else if (!isFavorite && existing != null)
            {
                _context.Favorites.Remove(existing);
            }

            await _context.SaveChangesAsync();
        }

        public async Task AddAsync(Meal entity)
        {
            _context.Meals.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Meal entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var meal = await _context.Meals.FindAsync(id);
            if (meal != null)
            {
                _context.Meals.Remove(meal);
                await _context.SaveChangesAsync();
            }
        }
    }
}