using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary.Data;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary.Repositories;

public class IngredientRepository : IIngredientRepository
{
    private readonly AppDbContext context;

    public IngredientRepository(AppDbContext context)
    {
        this.context = context;
    }

    public async Task<int> GetOrCreateIngredientIdByNameAsync(string name)
    {
        var ingredient = await context.Ingredients
            .FirstOrDefaultAsync(i => i.Name.ToLower() == name.ToLower());

        if (ingredient != null) return ingredient.IngredientId;

        var newIngredient = new Ingredient { Name = name };
        await context.Ingredients.AddAsync(newIngredient);
        await context.SaveChangesAsync();

        return newIngredient.IngredientId;
    }

    public async Task<List<KeyValuePair<int, string>>> SearchIngredientsAsync(string search)
    {
        return await context.Ingredients
            .Where(i => EF.Functions.Like(i.Name, $"%{search}%"))
            .OrderBy(i => i.Name)
            .Take(20)
            .Select(i => new KeyValuePair<int, string>(i.IngredientId, i.Name))
            .ToListAsync();
    }

    public async Task<List<Ingredient>> GetAllAsync()
    {
        return await context.Ingredients.OrderBy(i => i.Name).ToListAsync();
    }
}