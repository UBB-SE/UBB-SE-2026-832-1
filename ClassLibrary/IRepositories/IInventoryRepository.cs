using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClassLibrary.Models;

namespace ClassLibrary.IRepositories;

public interface IInventoryRepository
{
    Task AddAsync(Inventory entity, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Inventory>> GetAllByUserIdAsync(int userId, CancellationToken cancellationToken = default);

    Task<Inventory?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task UpdateAsync(Inventory entity, CancellationToken cancellationToken = default);
}
