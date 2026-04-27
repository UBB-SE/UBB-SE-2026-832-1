using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClassLibrary.Data;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary.Repositories;

public sealed class MealRepository(AppDbContext dbContext) : IMealRepository
{
    public async Task<Meal?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Meals
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Meal>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Meals
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Meal>> GetFilteredMealsAsync(MealFilter filter, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Meals.AsNoTracking().AsQueryable();

        if (filter.IsKeto)
        {
            query = query.Where(m => m.IsKeto);
        }

        if (filter.IsVegan)
        {
            query = query.Where(m => m.IsVegan);
        }

        if (filter.IsNutFree)
        {
            query = query.Where(m => m.IsNutFree);
        }

        if (filter.IsLactoseFree)
        {
            query = query.Where(m => m.IsLactoseFree);
        }

        if (filter.IsGlutenFree)
        {
            query = query.Where(m => m.IsGlutenFree);
        }

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            query = query.Where(m => m.Name.Contains(filter.SearchTerm));
        }

        var meals = await query.ToListAsync(cancellationToken);

        if (filter.IsFavoriteOnly)
        {
            var favoriteMealIds = await dbContext.Favorites
                .AsNoTracking()
                .Select(f => f.MealId)
                .ToListAsync(cancellationToken);

            meals = meals.Where(m => favoriteMealIds.Contains(m.Id)).ToList();
        }

        foreach (var meal in meals)
        {
            var ingredients = await dbContext.MealsIngredients
                .AsNoTracking()
                .Where(mi => mi.MealId == meal.Id)
                .Join(dbContext.Ingredients.AsNoTracking(),
                    mi => mi.FoodId,
                    i => i.FoodId,
                    (mi, i) => new { mi.Quantity, i.CaloriesPer100g, i.ProteinPer100g, i.CarbsPer100g, i.FatPer100g })
                .ToListAsync(cancellationToken);

            meal.Calories = (int)ingredients.Sum(x => x.CaloriesPer100g * x.Quantity / 100.0);
            meal.Protein = (int)ingredients.Sum(x => x.ProteinPer100g * x.Quantity / 100.0);
            meal.Carbs = (int)ingredients.Sum(x => x.CarbsPer100g * x.Quantity / 100.0);
            meal.Fat = (int)ingredients.Sum(x => x.FatPer100g * x.Quantity / 100.0);
        }

        return meals;
    }

    public async Task AddAsync(Meal entity, CancellationToken cancellationToken = default)
    {
        dbContext.Meals.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Meal entity, CancellationToken cancellationToken = default)
    {
        dbContext.Meals.Update(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.Meals.FindAsync([id], cancellationToken);
        if (entity is not null)
        {
            dbContext.Meals.Remove(entity);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task SetFavoriteAsync(int userId, int mealId, bool isFavorite, CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.Favorites
            .FirstOrDefaultAsync(f => f.UserId == userId && f.MealId == mealId, cancellationToken);

        if (isFavorite && existing is null)
        {
            dbContext.Favorites.Add(new Favorite { UserId = userId, MealId = mealId });
        }
        else if (!isFavorite && existing is not null)
        {
            dbContext.Favorites.Remove(existing);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<string>> GetIngredientLinesForMealAsync(int mealId, CancellationToken cancellationToken = default)
    {
        var ingredients = await dbContext.MealsIngredients
            .AsNoTracking()
            .Where(mi => mi.MealId == mealId)
            .Join(dbContext.Ingredients.AsNoTracking(),
                mi => mi.FoodId,
                i => i.FoodId,
                (mi, i) => new { i.Name, mi.Quantity })
            .OrderByDescending(x => x.Quantity)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return ingredients
            .Select(x => $"- {x.Name} ({x.Quantity:0.#}g)")
            .ToList();
    }
}
