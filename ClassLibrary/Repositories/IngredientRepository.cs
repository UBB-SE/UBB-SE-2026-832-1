using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary.Data;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary.Repositories;

/// <summary>
/// Entity Framework Core implementation of the Ingredient repository.
/// </summary>
public class IngredientRepository : IIngredientRepository
{
    private readonly AppDbContext _context;

    public IngredientRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> GetOrCreateIngredientIdAsync(string name)
    {
        return await GetOrCreateIngredientIdByNameAsync(name);
    }

    public async Task<int> GetOrCreateIngredientIdByNameAsync(string name)
    {
       
        var ingredient = await _context.Ingredients
            .FirstOrDefaultAsync(i => i.Name.ToLower() == name.ToLower());

        if (ingredient != null)
        {
            return ingredient.FoodId;
        }


        var newIngredient = new Ingredient
        {
            Name = name,
            CaloriesPer100g = 0,
            ProteinPer100g = 0,
            CarbohydratesPer100g = 0,
            FatPer100g = 0
        };

        await _context.Ingredients.AddAsync(newIngredient);
        await _context.SaveChangesAsync();

        return newIngredient.FoodId;
    }

    public async Task<List<KeyValuePair<int, string>>> SearchIngredientsAsync(string search)
    {
        return await _context.Ingredients
            .Where(i => EF.Functions.Like(i.Name, $"%{search}%"))
            .OrderBy(i => i.Name)
            .Take(20)
            .Select(i => new KeyValuePair<int, string>(i.FoodId, i.Name))
            .ToListAsync();
    }

    public async Task<List<Ingredient>> GetAllAsync()
    {
        return await _context.Ingredients
            .OrderBy(i => i.Name)
            .ToListAsync();
    }
}