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
    private readonly AppDbContext databaseContext;

    public IngredientRepository(AppDbContext databaseContext)
    {
        this.databaseContext = databaseContext;
    }

    public async Task<int> GetOrCreateIngredientIdByNameAsync(string name)
    {
        var ingredientEntity = await databaseContext.Ingredients
            .FirstOrDefaultAsync(ingredient => ingredient.Name.ToLower() == name.ToLower());

        if (ingredientEntity != null) return ingredientEntity.IngredientId;

        var newIngredient = new Ingredient { Name = name };
        await databaseContext.Ingredients.AddAsync(newIngredient);
        await databaseContext.SaveChangesAsync();

        return newIngredient.IngredientId;
    }

    public async Task<List<KeyValuePair<int, string>>> SearchIngredientsAsync(string search)
    {
        return await databaseContext.Ingredients
            .Where(ingredient => EF.Functions.Like(ingredient.Name, $"%{search}%"))
            .OrderBy(ingredient => ingredient.Name)
            .Take(20)
            .Select(ingredient => new KeyValuePair<int, string>(ingredient.IngredientId, ingredient.Name))
            .ToListAsync();
    }

    public async Task<List<Ingredient>> GetAllAsync()
    {
        return await databaseContext.Ingredients
            .OrderBy(ingredient => ingredient.Name)
            .ToListAsync();
    }
}