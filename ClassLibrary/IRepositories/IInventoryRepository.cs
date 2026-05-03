using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Models;

namespace ClassLibrary.IRepositories;

public interface IInventoryRepository
{
    Task<IReadOnlyList<Inventory>> GetAllByUserIdAsync(int userId);

    Task AddAsync(Inventory inventory);

    Task UpdateAsync(Inventory inventory);

    Task DeleteAsync(int inventoryId);
}
