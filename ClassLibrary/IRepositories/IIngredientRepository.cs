using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClassLibrary.Models;

namespace ClassLibrary.IRepositories;

public interface IIngredientRepository
{
    Task<IReadOnlyList<Ingredient>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<int> GetOrCreateIngredientIdAsync(string name, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<KeyValuePair<int, string>>> SearchIngredientsAsync(string search, CancellationToken cancellationToken = default);
}
