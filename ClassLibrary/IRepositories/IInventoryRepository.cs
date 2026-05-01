using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClassLibrary.Models;

namespace ClassLibrary.IRepositories;

public interface IInventoryRepository
{
    Task<IReadOnlyList<Inventory>> GetAllByUserIdAsync(int userId, CancellationToken cancellationToken = default);

    Task AddAsync(Inventory inventory, CancellationToken cancellationToken = default);

    Task UpdateAsync(Inventory inventory, CancellationToken cancellationToken = default);

    Task DeleteAsync(int inventoryId, CancellationToken cancellationToken = default);
}
