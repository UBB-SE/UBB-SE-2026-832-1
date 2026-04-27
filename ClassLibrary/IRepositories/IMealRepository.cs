using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClassLibrary.Models;

namespace ClassLibrary.IRepositories;

public interface IMealRepository
{
    Task AddAsync(Meal entity, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Meal>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Meal?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Meal>> GetFilteredMealsAsync(MealFilter filter, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> GetIngredientLinesForMealAsync(int mealId, CancellationToken cancellationToken = default);

    Task SetFavoriteAsync(int userId, int mealId, bool isFavorite, CancellationToken cancellationToken = default);

    Task UpdateAsync(Meal entity, CancellationToken cancellationToken = default);
}
