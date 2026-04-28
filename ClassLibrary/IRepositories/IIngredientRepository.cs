using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Models;

namespace ClassLibrary.IRepositories;

public interface IIngredientRepository
{
    Task<int> GetOrCreateIngredientIdByNameAsync(string name);
    Task<List<KeyValuePair<int, string>>> SearchIngredientsAsync(string search);
    Task<List<Ingredient>> GetAllAsync();
}