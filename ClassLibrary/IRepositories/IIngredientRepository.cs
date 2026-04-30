using ClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.IRepositories
{
    public interface IIngredientRepository
    {
        Task<int> GetOrCreateIngredientIdByNameAsync(string name);
        Task<List<KeyValuePair<int, string>>> SearchIngredientsAsync(string search);
        Task<List<Ingredient>> GetAllAsync();
    }
}
