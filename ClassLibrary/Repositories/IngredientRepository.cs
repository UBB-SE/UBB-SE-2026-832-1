using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClassLibrary.Data;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary.Repositories;

public sealed class IngredientRepository(AppDbContext dbContext) : IIngredientRepository
{
    public async Task<IReadOnlyList<Ingredient>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Ingredients
            .AsNoTracking()
            .OrderBy(i => i.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetOrCreateIngredientIdAsync(string name, CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.Ingredients
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Name.ToLower() == name.ToLower(), cancellationToken);

        if (existing is not null)
        {
            return existing.FoodId;
        }

        var ingredient = new Ingredient
        {
            Name = name,
            CaloriesPer100g = 0,
            ProteinPer100g = 0,
            CarbsPer100g = 0,
            FatPer100g = 0
        };

        dbContext.Ingredients.Add(ingredient);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ingredient.FoodId;
    }

    public async Task<IReadOnlyList<KeyValuePair<int, string>>> SearchIngredientsAsync(string search, CancellationToken cancellationToken = default)
    {
        var results = await dbContext.Ingredients
            .AsNoTracking()
            .Where(i => i.Name.Contains(search))
            .OrderBy(i => i.Name)
            .Take(20)
            .Select(i => new { i.FoodId, i.Name })
            .ToListAsync(cancellationToken);

        return results
            .Select(r => new KeyValuePair<int, string>(r.FoodId, r.Name))
            .ToList();
    }
}
