using ClassLibrary.Models;

namespace ClassLibrary.IRepositories
{
    public interface IShoppingListRepository
    {
        Task AddAsync(ShoppingItem item, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ShoppingItem>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ShoppingItem>> GetAllByUserIdAsync(int userId, CancellationToken cancellationToken = default);
        Task<ShoppingItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<ShoppingItem?> GetByUserAndIngredientAsync(int userId, int ingredientId, CancellationToken cancellationToken = default);
        Task UpdateAsync(ShoppingItem item, CancellationToken cancellationToken = default);
    }
}